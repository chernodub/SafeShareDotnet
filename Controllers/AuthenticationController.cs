using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using SafeShare.DAL;
using SafeShare.DTO;
using SafeShare.Models;
using SafeShare.Services;

namespace SafeShare.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationTokenService _authenticationTokenService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUserRepository _userRepository;

    public AuthenticationController(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IAuthenticationTokenService authenticationTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _authenticationTokenService = authenticationTokenService;
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

        await _userRepository.InsertUser(user);

        return Ok(new UserDto(user));
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        User? user = await _userRepository.GetUserByEmail(loginDto.Email);

        ValidateLoginDto(loginDto, user);

        if (!ModelState.IsValid || user is null)
        {
            return BadRequest(ModelState);
        }

        return Ok(CreateUserSecret(user));
    }

    private UserSecretDto CreateUserSecret(User user)
    {
        return new UserSecretDto { Token = _authenticationTokenService.GenerateToken(CreateClaimsPrincipal(user)) };
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(User user)
    {
        List<Claim> claims = new() { new Claim(ClaimTypes.Email, user.Email) };

        ClaimsIdentity claimsIdentity = new(claims, JwtBearerDefaults.AuthenticationScheme);
        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);

        return claimsPrincipal;
    }

    private void ValidateLoginDto(LoginDto loginDto, User? user)
    {
        bool isUserFound = user != null;
        bool isPasswordValid = user?.HashedPassword != null &&
                               _passwordHasher.VerifyHashedPassword(
                                   user, user.HashedPassword, loginDto.Password) == PasswordVerificationResult.Success;


        if (!isUserFound || !isPasswordValid)
        {
            ModelState.AddModelError(nameof(LoginDto.Email), "Invalid email or password");
        }
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
        if (await _userRepository.CheckIsUserPresentByEmail(registrationDto.Email))
        {
            ModelState.AddModelError(nameof(RegistrationDto.Email), "User already exists");
        }

        if (registrationDto.Password != registrationDto.ConfirmPassword)
        {
            ModelState.AddModelError(nameof(RegistrationDto.ConfirmPassword), "Passwords do not match");
        }
    }
}

