using SafeShare.Models;

namespace SafeShare.DTO;

public record MessageDto()
{
    public MessageDto(Message message) : this()
    {
        Text = message.Text;
        CreatedAt = message.Created.DateTime;
        OwnerEmail = message.OwnerEmail;
        ExpiresAt = message.ExpiresAt?.DateTime;
        IsOneTimeUse = message.IsOneTimeUse;
        Id = message.Id.ToString();
    }

    public string Id { get; }
    public bool IsOneTimeUse { get; }
    public string Text { get; }
    public DateTime CreatedAt { get; }
    public string OwnerEmail { get; }
    public DateTime? ExpiresAt { get; }
}