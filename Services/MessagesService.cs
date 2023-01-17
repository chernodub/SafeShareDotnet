using SafeShare.DAL;
using SafeShare.DTO;
using SafeShare.Exceptions;
using SafeShare.Models;

namespace SafeShare.Services;

public class MessagesService : IMessagesService
{
    private readonly IMessageRepository _messageRepository;

    public MessagesService(
        IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<List<MessageDto>> GetMessagesByUserEmail(string email)
    {
        List<Message> messages = await _messageRepository.GetMessagesByUserEmail(email);
        return messages.Select(m => new MessageDto(m)).ToList();
    }

    public async Task<MessageDto> CreateMessage(CreateMessageDto message, string ownerEmail)
    {
        Message newMessage = new()
        {
            Text = message.Text,
            ExpiresAt = message.ExpiresAt,
            IsOneTimeUse = message.IsOneTimeUse,
            OwnerEmail = ownerEmail
        };
        return new MessageDto(await _messageRepository.AddMessage(newMessage));
    }

    public async Task<MessageDto> ReadMessage(string id)
    {
        Message? message = await _messageRepository.GetMessageById(id);

        if (message == null)
        {
            throw new NotFoundException();
        }

        if (message.IsOneTimeUse)
        {
            await _messageRepository.DeleteMessage(message);
        }

        return new MessageDto(message);
    }

    public async Task DeleteMessage(string id)
    {
        Message? message = await _messageRepository.GetMessageById(id);

        if (message is null)
        {
            throw new NotFoundException();
        }
    }
}