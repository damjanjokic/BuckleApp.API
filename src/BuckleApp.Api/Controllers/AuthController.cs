using BuckleApp.Application.Features.Authentication;
using BuckleApp.Application.Features.Authentication.LoginUser;
using BuckleApp.Application.Features.Authentication.RegisterUser;
using BuckleApp.Application.Features.Authentication.ResetPassword;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using BuckleApp.Application.Features.Authentication.RequestStudentLoginCode;
using BuckleApp.Application.Features.Authentication.RefreshSession;
using BuckleApp.Application.Features.Authentication.LogoutUser;


namespace BuckleApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[EnableRateLimiting("authentication")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [Authorize(Roles = "SuperAdmin,Admin,Instructor")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var response = await _authService.RegisterUser(request);

        return Ok(response);
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginUserRequest request)
    {
        var response = await _authService.LoginUser(request);

        return Ok(response);
    }
    
    [HttpPost("password-reset")]
    public async Task<IActionResult> LoginAsync([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var response = await _authService.ResetPassword(request);

        return Ok(response);
    }
    
    [AllowAnonymous]
    [HttpPost("student/request-code")]
    public async Task<IActionResult> RequestCode([FromBody] RequestStudentLoginCodeRequest request)
    {
        await _authService.RequestStudentLoginCode(request);
        return Ok(new { Message = "If the account is eligible, a login code will be sent." });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshSessionRequest request)
        => Ok(await _authService.RefreshSession(request));

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutUserRequest request)
    {
        await _authService.LogoutUser(request);
        return Ok();
    }
}
