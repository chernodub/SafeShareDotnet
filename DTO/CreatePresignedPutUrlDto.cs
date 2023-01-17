namespace SafeShare.DTO;

public record CreatePresignedPutUrlDto

{
    public string Name { get; init; }

    /// <summary>
    ///     Date and time when the message should be deleted.
    /// </summary>
    public DateTime ExpiresAt { get; init; }

    /// <summary>
    ///     If `true`, the resource will be deleted after the first read.
    /// </summary>
    public bool IsOneTimeUse { get; init; }
}