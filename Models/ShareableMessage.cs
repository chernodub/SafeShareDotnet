namespace SafeShare.Models;

public class ShareableMessage : ShareableResource
{
    public required string Text { get; init; }
}