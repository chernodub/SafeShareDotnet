using SafeShare.Models;

namespace SafeShare.DAL;

public interface IMessageRepository
{
    public Task<List<Message>> GetMessagesByUserEmail(string email);
    public Task<Message?> GetMessageById(string id);
    public Task<Message> AddMessage(Message message);
    public Task DeleteMessage(Message message);
}