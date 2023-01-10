using SafeShare.Models;

namespace SafeShare.Dto;

public record ShareableMessageDto()
{
    public ShareableMessageDto(ShareableMessage message) : this()
    {
        Text = message.Text;
        CreatedAt = message.Created.DateTime;
        OwnerEmail = message.OwnerEmail;
        ExpiresAt = message.ExpirationDate.DateTime;
    }

    public string Text { get; init; }
    public DateTime CreatedAt { get; init; }
    public string OwnerEmail { get; init; }
    public DateTime ExpiresAt { get; init; }
}