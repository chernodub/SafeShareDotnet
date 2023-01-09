using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace SafeShare.Services;

public class JwtAuthenticationTokenService : IAuthenticationTokenService
{
    private const string JwtSecurityAlgorithm = SecurityAlgorithms.HmacSha256;
    private readonly IConfiguration _configuration;

    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

    public JwtAuthenticationTokenService(
        JwtSecurityTokenHandler jwtSecurityTokenHandler,
        IConfiguration configuration
    )
    {
        _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        _configuration = configuration;
    }

    public string GenerateToken(ClaimsPrincipal claimsPrincipal)
    {
        const int jwtExpirationMinutes = 1;

        JwtSecurityToken token = new(
            claims: claimsPrincipal.Claims,
            expires: DateTime.UtcNow.AddMinutes(jwtExpirationMinutes),
            signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(_configuration), JwtSecurityAlgorithm));

        return _jwtSecurityTokenHandler.WriteToken(token);
    }

    public static TokenValidationParameters GetTokenValidationParameters(IConfiguration configuration)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = GetSymmetricSecurityKey(configuration),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    }

    private static SymmetricSecurityKey GetSymmetricSecurityKey(IConfiguration configuration)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWT_SECRET")!));
    }
}

