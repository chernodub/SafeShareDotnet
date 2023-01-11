using Microsoft.EntityFrameworkCore;

using SafeShare.Models;

namespace SafeShare.DAL;

public class MessageRepository : IMessageRepository

{
    private readonly MessagesContext _messagesContext;

    public MessageRepository(
        MessagesContext messagesContext)
    {
        _messagesContext = messagesContext;
    }

    public Task<List<Message>> GetMessagesByUserEmail(string email)
    {
        return _messagesContext.Messages
            .Where(m => m.OwnerEmail == email)
            .ToListAsync();
    }

    public async Task AddMessage(Message message)
    {
        await _messagesContext.Messages.AddAsync(message);
        await _messagesContext.SaveChangesAsync();
    }

    public async Task RemoveMessage(Message message)
    {
        _messagesContext.Messages.Remove(message);
        await _messagesContext.SaveChangesAsync();
    }

    public Task<Message?> GetMessageById(string id)
    {
        return _messagesContext.Messages.Include(m => m.Owner)
            .FirstOrDefaultAsync(m => m.Id.Equals(new Guid(id)));
    }
}