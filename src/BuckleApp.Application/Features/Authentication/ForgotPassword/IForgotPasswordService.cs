namespace BuckleApp.Application.Features.Authentication.ForgotPassword;

public interface IForgotPasswordService
{
    Task ExecuteAsync(ForgotPasswordRequest request);
}