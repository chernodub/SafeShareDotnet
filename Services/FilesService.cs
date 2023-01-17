using SafeShare.DAL;
using SafeShare.DTO;
using SafeShare.Exceptions;

using File = SafeShare.Models.File;

namespace SafeShare.Services;

public class FilesService : IFilesService
{
    private const int PresignedRequestDurationSeconds = 60 * 60; // 1 hour
    private readonly BlobRepository _blobRepository;
    private readonly string _bucketName;
    private readonly IFilesRepository _filesRepository;

    public FilesService(
        BlobRepository blobRepository,
        IConfiguration configuration,
        IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
        _bucketName = configuration["MINIO_BUCKET_NAME"]!;
        _blobRepository = blobRepository;
    }

    public async Task<List<FileDto>> GetFilesByEmail(string email)
    {
        File[] files = await _filesRepository.GetFilesByUserEmail(email);
        IEnumerable<FileDto> fileDtos = files.Select(file => new FileDto(file));
        return fileDtos.ToList();
    }

    public async Task<string> CreatePresignedGetUrl(string id)
    {
        File? file = await _filesRepository.GetFileById(id);
        if (file is null)
        {
            throw new NotFoundException();
        }

        string urlString =
            await _blobRepository.GeneratePresignedGetRequest(_bucketName, id, PresignedRequestDurationSeconds);

        if (file.IsOneTimeUse)
        {
            // TODO clean up the blob storage too
            await _filesRepository.RemoveFile(file);
        }

        return urlString;
    }

    public async Task<string> CreatePresignedPutUrl(string currentUserEmail, CreatePresignedPutUrlDto dto)
    {
        File file = await _filesRepository.AddFile(new File
        {
            Name = dto.Name,
            OwnerEmail = currentUserEmail,
            IsOneTimeUse = dto.IsOneTimeUse,
            ExpiresAt = dto.ExpiresAt
        });

        string urlString =
            await _blobRepository.GeneratePresignedPutRequest(
                _bucketName,
                dto.Name,
                file.Id.ToString(),
                PresignedRequestDurationSeconds);

        return urlString;
    }

    public async Task DeleteFile(string id)
    {
        File? file = await _filesRepository.GetFileById(id);
        if (file is null)
        {
            throw new NotFoundException();
        }

        await _filesRepository.RemoveFile(file);
        await _blobRepository.RemoveObject(_bucketName, id);
    }
}