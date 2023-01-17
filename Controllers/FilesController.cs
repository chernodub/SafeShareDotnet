using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SafeShare.DAL;
using SafeShare.Dto;

using File = SafeShare.Models.File;

namespace SafeShare.Controllers;

[Route("api/files")]
[Authorize]
[ApiController]
public class FilesController : ControllerBase
{
    private const int PresignedRequestDurationSeconds = 60 * 60; // 1 hour
    private readonly BlobRepository _blobRepository;
    private readonly string _bucketName;
    private readonly IFilesRepository _filesRepository;

    public FilesController(
        BlobRepository blobRepository,
        IConfiguration configuration,
        IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
        _bucketName = configuration["MINIO_BUCKET_NAME"]!;
        _blobRepository = blobRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetFiles()
    {
        string? userId = User.FindFirstValue(ClaimTypes.Email);
        if (userId is null)
        {
            return Unauthorized();
        }

        File[] files = await _filesRepository.GetFilesByUserEmail(userId);
        IEnumerable<FileDto> fileDtos = files.Select(file => new FileDto(file));
        return Ok(fileDtos);
    }

    [HttpPost("presigned-url")]
    public async Task<ActionResult> CreatePresignedPutUrl([FromBody] CreatePresignedPutUrlDto dto)
    {
        string? currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
        if (currentUserEmail is null)
        {
            return Unauthorized();
        }

        File file = await _filesRepository.AddFile(new File
        {
            Name = dto.Name,
            OwnerEmail = currentUserEmail,
            IsOneTimeUse = dto.IsOneTimeUse,
            ExpiresAt = dto.ExpiresAt
        });

        string urlString =
            await _blobRepository.GeneratePresignedPutRequest(
                _bucketName,
                dto.Name,
                file.Id.ToString(),
                PresignedRequestDurationSeconds);

        return Ok(urlString);
    }


    [HttpPost("read/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult> CreatePresignedGetUrl([FromRoute] string id)
    {
        File? file = await _filesRepository.GetFileById(id);
        if (file is null)
        {
            return NotFound();
        }

        string urlString =
            await _blobRepository.GeneratePresignedGetRequest(_bucketName, id, PresignedRequestDurationSeconds);

        if (file.IsOneTimeUse)
        {
            // TODO clean up the blob storage too
            await _filesRepository.RemoveFile(file);
        }

        return Redirect(urlString);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteFile([FromRoute] string id)
    {
        string? email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null)
        {
            return Unauthorized();
        }

        File? file = await _filesRepository.GetFileById(id);
        if (file is null)
        {
            return NotFound();
        }

        if (file.OwnerEmail != email)
        {
            return Unauthorized();
        }

        await _filesRepository.RemoveFile(file);
        await _blobRepository.RemoveObject(_bucketName, id);

        return Ok();
    }
}