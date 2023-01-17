using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SafeShare.DTO;
using SafeShare.Exceptions;
using SafeShare.Services;

namespace SafeShare.Controllers;

[Route("api/messages")]
[Authorize]
[ApiController]
public class MessagesController : ControllerBase
{
    private readonly IMessagesService _messagesService;

    public MessagesController(
        IMessagesService messagesService)
    {
        _messagesService = messagesService;
    }

    /// <summary>
    ///     Returns messages for the current authorized user.
    /// </summary>
    /// <returns></returns>
    [HttpGet("my")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDto[]))]
    public async Task<IActionResult> GetMessages()
    {
        string? email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null)
        {
            return Unauthorized();
        }

        return Ok(await _messagesService.GetMessagesByUserEmail(email));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MessageDto))]
    public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDto message)
    {
        string? ownerEmail = User.FindFirstValue(ClaimTypes.Email);
        if (ownerEmail is null)
        {
            return Unauthorized();
        }

        return CreatedAtAction(nameof(GetMessages), await _messagesService.CreateMessage(message, ownerEmail));
    }

    /// <summary>
    ///     Returns the message and deletes it if it's has a limited lifecycle.
    /// </summary>
    /// <param name="id">Id of the message.</param>
    /// <returns></returns>
    [HttpPost("read/{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReadMessage([FromRoute] string id)
    {
        try
        {
            MessageDto messageDto = await _messagesService.ReadMessage(id);

            return Ok(messageDto);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteMessage([FromRoute] string id)
    {
        try
        {
            await _messagesService.DeleteMessage(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}