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

    [HttpGet("my")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ShareableMessageDto>))]
    public async Task<IActionResult> GetUploadedMessages()
    {
        string? email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null)
        {
            return Unauthorized();
        }

        List<ShareableMessage> messages = await _shareableMessageRepository.GetMessagesByUserEmail(email);
        List<ShareableMessageDto> dtos = messages.Select(m => new ShareableMessageDto(m)).ToList();

        return Ok(dtos);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ShareableMessageDto))]
    public async Task<IActionResult> UploadMessage([FromBody] CreateShareableMessageDto message)
    {
        string? email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null)
        {
            return Unauthorized();
        }

        ShareableMessage newMessage = new()
        {
            Text = message.Text,
            ExpiresAt = message.ExpiresAt,
            IsOneTimeUse = message.IsOneTimeUse,
            OwnerEmail = email
        };
        await _shareableMessageRepository.AddMessage(newMessage);

        return CreatedAtAction(nameof(GetUploadedMessages), new ShareableMessageDto(newMessage));
    }

    [HttpPost("read/{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ShareableMessageDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReadMessage([FromRoute] string id)
    {
        ShareableMessage? message = await _shareableMessageRepository.GetMessageById(id);

        if (message is null)
        {
            return NotFound();
        }

        if (message.IsOneTimeUse)
        {
            await _shareableMessageRepository.RemoveMessage(message);
        }

        return Ok(new ShareableMessageDto(message));
    }
}