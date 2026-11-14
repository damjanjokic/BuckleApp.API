using System.ComponentModel.DataAnnotations;
namespace BuckleApp.Application.Features.Authentication.VerifyStudentLoginCode;

public sealed record VerifyStudentLoginCodeRequest
{
    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = null!;

    [Required, RegularExpression("^[0-9]{6}$")]
    public string Code { get; set; } = null!;
}
