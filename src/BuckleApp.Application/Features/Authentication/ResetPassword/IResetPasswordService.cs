namespace BuckleApp.Application.Features.Authentication.ResetPassword;

public interface IResetPasswordService
{
    Task<bool> ExecuteAsync(ResetPasswordRequest request);
}