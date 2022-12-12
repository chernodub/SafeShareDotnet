using System.ComponentModel.DataAnnotations;

namespace SafeShare.Dto;

public record RegistrationDto
{
    public string ConfirmPassword { get; init; } = null!;
    [EmailAddress] public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;

    [MinLength(6)] public string Password { get; init; } = null!;
}