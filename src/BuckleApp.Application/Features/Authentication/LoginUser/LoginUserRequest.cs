using System.ComponentModel.DataAnnotations;

namespace BuckleApp.Application.Features.Authentication.LoginUser;

public sealed record LoginUserRequest
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}