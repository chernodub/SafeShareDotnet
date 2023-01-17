using File = SafeShare.Models.File;

namespace SafeShare.DTO;

public record FileDto
{
    public FileDto(File file)
    {
        Id = file.Id.ToString();
        Name = file.Name;
        IsOneTimeUse = file.IsOneTimeUse;
        CreatedAt = file.Created.DateTime;
        OwnerEmail = file.OwnerEmail;
        ExpiresAt = file.ExpiresAt?.DateTime;
    }

    public string Id { get; }
    public bool IsOneTimeUse { get; }
    public string Name { get; }
    public DateTime CreatedAt { get; }
    public string OwnerEmail { get; }
    public DateTime? ExpiresAt { get; }
}