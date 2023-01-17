using SafeShare.DTO;

namespace SafeShare.Services;

public interface IFilesService
{
    public Task<List<FileDto>> GetFilesByEmail(string email);
    public Task<string> CreatePresignedGetUrl(string id);
    public Task<string> CreatePresignedPutUrl(string currentUserEmail, CreatePresignedPutUrlDto dto);
    public Task DeleteFile(string id);
}