using System.ComponentModel.DataAnnotations;

namespace SafeShare.Models;

public class User
{
    [Key] [EmailAddress] public required string Email { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public string? HashedPassword { get; set; }
}
