using System.Security.Claims;

namespace SafeShare.Services;

public interface IAuthenticationTokenService
{
    public string GenerateToken(ClaimsPrincipal claimsPrincipal);
}