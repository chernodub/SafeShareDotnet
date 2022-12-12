using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SafeShare.Dto;
using SafeShare.Models;

namespace SafeShare.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly AppDbContext _appDbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthenticationController(
        AppDbContext appDbContext,
        IPasswordHasher<User> passwordHasher
    )
    {
        _appDbContext = appDbContext;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegistrationDto registrationDto)
    {
        await ValidateRegistrationDto(registrationDto);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        User user = CreateUser(registrationDto);

        await _appDbContext.Users.AddAsync(user);
        await _appDbContext.SaveChangesAsync();

        return Ok(new UserDto(user));
    }

    private User CreateUser(RegistrationDto registrationDto)
    {
        User user = new()
        {
            Email = registrationDto.Email,
            FirstName = registrationDto.FirstName,
            LastName = registrationDto.LastName
        };
        user.HashedPassword = _passwordHasher.HashPassword(user, registrationDto.Password);

        return user;
    }

    private async Task ValidateRegistrationDto(RegistrationDto registrationDto)
    {
        await CheckIsUserPresent(registrationDto);

        if (registrationDto.Password != registrationDto.ConfirmPassword)
        {
            ModelState.AddModelError(nameof(RegistrationDto.ConfirmPassword), "Passwords do not match");
        }
    }

    private async Task CheckIsUserPresent(RegistrationDto registrationDto)
    {
        if (await _appDbContext.Users.AnyAsync(user => user.Email == registrationDto.Email))
        {
            ModelState.AddModelError(nameof(RegistrationDto.Email), "User already exists");
        }
    }
}
