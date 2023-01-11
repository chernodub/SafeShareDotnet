namespace SafeShare.Dto;

public record CreateMessageDto
{
    /// <summary>
    ///     Date and time when the message should be deleted.
    /// </summary>
    public DateTime ExpiresAt { get; init; }

    /// <summary>
    ///     If `true`, the resource will be deleted after the first read.
    /// </summary>
    public bool IsOneTimeUse { get; init; }

    /// <summary>
    ///     Message text
    /// </summary>
    public string Text { get; init; }
}