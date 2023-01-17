using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using SafeShare.Models;

using File = SafeShare.Models.File;

namespace SafeShare.DAL;

public class FilesRepository : IFilesRepository
{
    private readonly MessagesContext _messagesContext;

    public FilesRepository(
        MessagesContext messagesContext)
    {
        _messagesContext = messagesContext;
    }

    public Task<File[]> GetFilesByUserEmail(string email)
    {
        return _messagesContext.Files
            .Where(f => f.OwnerEmail == email)
            .ToArrayAsync();
    }

    public async Task<File> AddFile(File file)
    {
        EntityEntry<File> entry = await _messagesContext.Files.AddAsync(file);
        await _messagesContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task RemoveFile(File file)
    {
        _messagesContext.Files.Remove(file);
        await _messagesContext.SaveChangesAsync();
    }

    public Task<File?> GetFileById(string id)
    {
        return _messagesContext.Files.FirstOrDefaultAsync(f => f.Id.ToString() == id);
    }
}