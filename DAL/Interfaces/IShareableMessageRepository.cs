using SafeShare.Models;

namespace SafeShare.DAL;

public interface IShareableMessageRepository
{
    public Task<List<ShareableMessage>> GetMessages();
    public Task<ShareableMessage?> GetMessageById(int id);
    public Task AddMessage(ShareableMessage message);
}