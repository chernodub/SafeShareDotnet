using System.ComponentModel.DataAnnotations;

namespace SafeShare.Models;

public abstract class ShareableResource
{
    [Key] public int Id { get; init; }
    public DateTime Created { get; init; } = DateTime.Now;
    public required DateTime ExpirationDate { get; init; }

    public required User Owner { get; init; }
}