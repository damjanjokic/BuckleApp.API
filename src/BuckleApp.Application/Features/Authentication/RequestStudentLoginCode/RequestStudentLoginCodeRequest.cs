using System.ComponentModel.DataAnnotations;
namespace BuckleApp.Application.Features.Authentication.RequestStudentLoginCode;

public sealed record RequestStudentLoginCodeRequest
{
    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = null!;
}
