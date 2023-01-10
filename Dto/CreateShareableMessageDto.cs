namespace SafeShare.Dto;

public record CreateShareableMessageDto
{
    public DateTime ExpiresAt { get; init; }
    public string Text { get; init; }
}