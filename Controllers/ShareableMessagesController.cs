using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SafeShare.DAL;
using SafeShare.Dto;
using SafeShare.Models;

namespace SafeShare.Controllers;

[Route("api/messages")]
[Authorize]
[ApiController]
public class ShareableMessagesController : ControllerBase
{
    private readonly IShareableMessageRepository _shareableMessageRepository;

    public ShareableMessagesController(
        IShareableMessageRepository shareableMessageRepository)
    {
        _shareableMessageRepository = shareableMessageRepository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ShareableMessage>))]
    public async Task<OkObjectResult> GetUploadedMessages()
    {
        return Ok(await _shareableMessageRepository.GetMessages());
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ShareableMessage))]
    public async Task<IActionResult> UploadMessage([FromBody] CreateShareableMessageDto message)
    {
        string? email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null)
        {
            return Unauthorized();
        }

        ShareableMessage newMessage = new()
        {
            Text = message.Text, ExpirationDate = new DateTimeOffset(message.ExpirationDate), OwnerEmail = email
        };
        await _shareableMessageRepository.AddMessage(newMessage);

        return CreatedAtAction(nameof(GetUploadedMessages), newMessage);
    }

    [HttpPost("read/{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ShareableMessage))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessage([FromRoute] string id)
    {
        int numericId = Int32.Parse(id);
        ShareableMessage? message = await _shareableMessageRepository.GetMessageById(numericId);

        if (message is null)
        {
            return NotFound();
        }

        await _shareableMessageRepository.RemoveMessage(message);

        return Ok(message);
    }
}