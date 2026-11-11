using BuckleApp.Application.Features.Authentication;
using BuckleApp.Application.Features.Authentication.LoginUser;
using BuckleApp.Application.Features.Authentication.RegisterUser;
using BuckleApp.Application.Features.Authentication.ResetPassword;
using Microsoft.AspNetCore.Mvc;

namespace BuckleApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var response = await _authService.RegisterUser(request);

        return Ok(response);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginUserRequest request)
    {
        var response = _authService.LoginUser(request);

        return Ok(response);
    }
    
    [HttpPost("password-reset")]
    public async Task<IActionResult> LoginAsync([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var response = await _authService.ResetPassword(request);

        return Ok(response);
    }
    
}