using SafeShare.DTO;

namespace SafeShare.Services;

public interface IMessagesService
{
    public Task<List<MessageDto>> GetMessagesByUserEmail(string email);
    public Task<MessageDto> CreateMessage(CreateMessageDto message, string ownerEmail);
    public Task<MessageDto> ReadMessage(string id);
    public Task DeleteMessage(string id);
}