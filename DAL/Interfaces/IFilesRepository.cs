using File = SafeShare.Models.File;

namespace SafeShare.DAL;

public interface IFilesRepository
{
    public Task<File[]> GetFilesByUserEmail(string email);
    public Task<File> AddFile(File file);
    public Task RemoveFile(File file);
    public Task<File?> GetFileById(string id);
}