using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

using SafeShare.DAL;
using SafeShare.DTO;
using SafeShare.Exceptions;
using SafeShare.Models;

namespace SafeShare.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationTokenService _authenticationTokenService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUserRepository _userRepository;

    public AuthenticationService(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IAuthenticationTokenService authenticationTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _authenticationTokenService = authenticationTokenService;
    }

    /// <summary>
    ///     Creates new user in the system.
    /// </summary>
    /// <param name="model">Data required to register a new user.</param>
    /// <exception cref="DataValidationException"></exception>
    /// <returns>Created user</returns>
    public async Task<UserDto> Register(RegistrationDto model)
    {
        await ValidateRegistrationDto(model);
        User user = CreateUser(model);

        await _userRepository.InsertUser(user);

        return new UserDto(user);
    }

    /// <summary>
    ///     Authenticates user and returns JWT token.
    /// </summary>
    /// <param name="model">Data for login.</param>
    /// <exception cref="DataValidationException"></exception>
    /// <returns></returns>
    public async Task<UserSecretDto> Login(LoginDto model)
    {
        User? user = await _userRepository.GetUserByEmail(model.Email);

        ValidateLoginDto(model, user);

        return CreateUserSecret(user!);
    }

    private async Task ValidateRegistrationDto(RegistrationDto registrationDto)
    {
        if (await _userRepository.CheckIsUserPresentByEmail(registrationDto.Email))
        {
            throw new DataValidationException(nameof(RegistrationDto.Email), "User already exists");
        }

        if (registrationDto.Password != registrationDto.ConfirmPassword)
        {
            throw new DataValidationException(nameof(RegistrationDto.ConfirmPassword), "Passwords do not match");
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

    private UserSecretDto CreateUserSecret(User user)
    {
        return new UserSecretDto { Token = _authenticationTokenService.GenerateToken(CreateClaimsPrincipal(user)) };
    }

    private void ValidateLoginDto(LoginDto loginDto, User? user)
    {
        bool isUserFound = user != null;
        bool isPasswordValid = user?.HashedPassword != null &&
                               _passwordHasher.VerifyHashedPassword(
                                   user, user.HashedPassword, loginDto.Password) == PasswordVerificationResult.Success;


        if (!isUserFound || !isPasswordValid)
        {
            throw new DataValidationException(nameof(LoginDto.Email), "Invalid email or password");
        }
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(User user)
    {
        List<Claim> claims = new() { new Claim(ClaimTypes.Email, user.Email) };

        ClaimsIdentity claimsIdentity = new(claims, JwtBearerDefaults.AuthenticationScheme);
        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);

        return claimsPrincipal;
    }
}
