using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SafeShare.DTO;
using SafeShare.Exceptions;
using SafeShare.Services;

namespace SafeShare.Controllers;

[Route("api/files")]
[Authorize]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IFilesService _filesService;

    public FilesController(
        IFilesService filesService)
    {
        _filesService = filesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFiles()
    {
        string? email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null)
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await _filesService.GetFilesByEmail(email));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("presigned-url")]
    public async Task<ActionResult> CreatePresignedPutUrl([FromBody] CreatePresignedPutUrlDto dto)
    {
        string? currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
        if (currentUserEmail is null)
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await _filesService.CreatePresignedPutUrl(currentUserEmail, dto));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }


    [HttpPost("read/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult> CreatePresignedGetUrl([FromRoute] string id)
    {
        try
        {
            return Redirect(await _filesService.CreatePresignedGetUrl(id));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteFile([FromRoute] string id)
    {
        string? email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null)
        {
            return Unauthorized();
        }

        try
        {
            await _filesService.DeleteFile(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}