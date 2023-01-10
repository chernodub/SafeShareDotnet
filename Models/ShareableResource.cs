using System.ComponentModel.DataAnnotations;

namespace SafeShare.Models;

public abstract class ShareableResource
{
    [Key] public int Id { get; init; }
    public DateTimeOffset Created { get; init; } = DateTimeOffset.Now;
    public required DateTimeOffset ExpirationDate { get; init; }
    public required string OwnerEmail { get; init; }

    public User Owner { get; init; }
}