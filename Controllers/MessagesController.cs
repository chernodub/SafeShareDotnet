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
public class MessagesController : ControllerBase
{
    private readonly IMessageRepository _messageRepository;

    public MessagesController(
        IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    [HttpGet("my")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MessageDto>))]
    public async Task<IActionResult> GetUploadedMessages()
    {
        string? email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null)
        {
            return Unauthorized();
        }

        List<Message> messages = await _messageRepository.GetMessagesByUserEmail(email);
        List<MessageDto> dtos = messages.Select(m => new MessageDto(m)).ToList();

        return Ok(dtos);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MessageDto))]
    public async Task<IActionResult> UploadMessage([FromBody] CreateMessageDto message)
    {
        string? email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null)
        {
            return Unauthorized();
        }

        Message newMessage = new()
        {
            Text = message.Text,
            ExpiresAt = message.ExpiresAt,
            IsOneTimeUse = message.IsOneTimeUse,
            OwnerEmail = email
        };
        await _messageRepository.AddMessage(newMessage);

        return CreatedAtAction(nameof(GetUploadedMessages), new MessageDto(newMessage));
    }

    [HttpPost("read/{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReadMessage([FromRoute] string id)
    {
        Message? message = await _messageRepository.GetMessageById(id);

        if (message is null)
        {
            return NotFound();
        }

        if (message.IsOneTimeUse)
        {
            await _messageRepository.RemoveMessage(message);
        }

        return Ok(new MessageDto(message));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteMessage([FromRoute] string id)
    {
        Message? message = await _messageRepository.GetMessageById(id);

        if (message is null)
        {
            return NotFound();
        }

        await _messageRepository.RemoveMessage(message);
        return NoContent();
    }
}