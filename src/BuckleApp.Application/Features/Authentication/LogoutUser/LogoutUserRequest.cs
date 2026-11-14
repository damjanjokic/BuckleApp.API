using System.ComponentModel.DataAnnotations;
namespace BuckleApp.Application.Features.Authentication.LogoutUser;

public sealed record LogoutUserRequest
{
    [Required, MaxLength(256)]
    public string RefreshToken { get; set; } = null!;
}
