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

    public Task<List<ShareableMessage>> GetMessagesByUserEmail(string email)
    {
        return _shareableResourcesContext.Messages
            .Where(m => m.OwnerEmail == email)
            .ToListAsync();
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

    public Task<ShareableMessage?> GetMessageById(string id)
    {
        return _shareableResourcesContext.Messages.Include(m => m.Owner)
            .FirstOrDefaultAsync(m => m.Id.Equals(new Guid(id)));
    }
}