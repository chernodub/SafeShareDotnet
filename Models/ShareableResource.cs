using System.ComponentModel.DataAnnotations;

namespace SafeShare.Models;

public abstract class ShareableResource
{
    [Key] public Guid Id { get; init; }
    public DateTimeOffset Created { get; init; } = DateTimeOffset.Now;

    /// <summary>
    ///     If present, the resource is going to be disposed after it's viewed.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>
    ///     If `true` the resource should be disposed after the first download.
    /// </summary>
    public bool IsOneTimeUse { get; init; }

    public required string OwnerEmail { get; init; }

    public User? Owner { get; init; }
}
