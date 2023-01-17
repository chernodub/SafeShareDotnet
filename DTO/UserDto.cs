using System.ComponentModel.DataAnnotations;

using SafeShare.Models;

namespace SafeShare.DTO;

public record UserDto
{
    public UserDto(User user)
    {
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
    }

    [Key] [EmailAddress] public string Email { get; }

    public string FirstName { get; }

    public string LastName { get; }
}