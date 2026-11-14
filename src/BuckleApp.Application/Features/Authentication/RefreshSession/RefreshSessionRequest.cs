using System.ComponentModel.DataAnnotations;
namespace BuckleApp.Application.Features.Authentication.RefreshSession;

public sealed record RefreshSessionRequest
{
    [Required, MaxLength(256)]
    public string RefreshToken { get; set; } = null!;
}
