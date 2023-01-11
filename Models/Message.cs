namespace SafeShare.Models;

public class Message : ShareableResource
{
    public required string Text { get; init; }
}