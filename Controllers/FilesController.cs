using Microsoft.AspNetCore.Mvc;

using SafeShare.Services;

namespace SafeShare.Controllers;

[Route("api/files")]
[ApiController]
public class FilesController : ControllerBase
{
    private const int PresignedRequestDurationSeconds = 60 * 60; // 1 hour
    private readonly BlobStorageService _blobStorageService;
    private readonly string _bucketName;

    public FilesController(
        BlobStorageService blobStorageService,
        IConfiguration configuration)
    {
        _bucketName = configuration["MINIO_BUCKET_NAME"]!;
        _blobStorageService = blobStorageService;
    }

    [HttpPost("presigned-url")]
    public async Task<ActionResult> GetPresignedUrlForUploadingFile([FromBody] string objectName)
    {
        string urlString =
            await _blobStorageService.GeneratePresignedPutRequest(_bucketName, objectName, Guid.NewGuid().ToString(),
                PresignedRequestDurationSeconds);

        return Ok(urlString);
    }


    [HttpGet("{name}")]
    public async Task<ActionResult> GetPresignedUrlForDownloadingFile(string name)
    {
        string urlString =
            await _blobStorageService.GeneratePresignedGetRequest(_bucketName, name, name,
                PresignedRequestDurationSeconds);

        return Redirect(urlString);
    }
}