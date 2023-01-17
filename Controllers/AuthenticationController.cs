using Microsoft.AspNetCore.Mvc;

using SafeShare.DTO;
using SafeShare.Exceptions;
using SafeShare.Services;

namespace SafeShare.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(
        IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegistrationDto registrationDto)
    {
        try
        {
            UserDto user = await _authenticationService.Register(registrationDto);
            return Ok(user);
        }
        catch (DataValidationException e)
        {
            ModelState.AddModelError(e.Field, e.Message);
            return BadRequest(ModelState);
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        try
        {
            UserSecretDto secret = await _authenticationService.Login(loginDto);
            return Ok(secret);
        }
        catch (DataValidationException e)
        {
            ModelState.AddModelError(e.Field, e.Message);
            return BadRequest(ModelState);
        }
    }
}

