using System.ComponentModel.DataAnnotations;

namespace SafeShare.DTO;

public record RegistrationDto
{
    public required string ConfirmPassword { get; init; }
    [EmailAddress] public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    [MinLength(6)] public required string Password { get; init; }
}