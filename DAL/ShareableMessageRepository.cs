using Microsoft.EntityFrameworkCore;

using SafeShare.Models;

namespace SafeShare.DAL;

public class ShareableMessageRepository : IShareableMessageRepository

{
    private readonly ShareableResourcesContext _shareableResourcesContext;

    public ShareableMessageRepository(
        ShareableResourcesContext shareableResourcesContext)
    {
        _shareableResourcesContext = shareableResourcesContext;
    }

    public Task<List<ShareableMessage>> GetMessages()
    {
        return _shareableResourcesContext.Messages.ToListAsync();
    }

    public Task<ShareableMessage?> GetMessageById(int id)
    {
        return _shareableResourcesContext.Messages.Include(m => m.Owner).FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task AddMessage(ShareableMessage message)
    {
        await _shareableResourcesContext.Messages.AddAsync(message);
        await _shareableResourcesContext.SaveChangesAsync();
    }

    public async Task RemoveMessage(ShareableMessage message)
    {
        _shareableResourcesContext.Messages.Remove(message);
        await _shareableResourcesContext.SaveChangesAsync();
    }
}