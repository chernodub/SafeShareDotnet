using SafeShare.DTO;

namespace SafeShare.Services;

public interface IAuthenticationService
{
    Task<UserDto> Register(RegistrationDto model);
    Task<UserSecretDto> Login(LoginDto model);
}