namespace SafeShare.Dto;

public record CreateShareableMessageDto
{
    public DateTime ExpirationDate { get; init; }
    public string Text { get; init; }
}