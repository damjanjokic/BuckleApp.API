using System.ComponentModel.DataAnnotations;

namespace BuckleApp.Application.Features.Authentication.ResetPassword;

public sealed record ResetPasswordRequest
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required]
    public string Token { get; set; }
    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}