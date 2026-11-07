namespace BuckleApp.Application.Features.Authentication.ForgotPassword;

public class ForgotPasswordService : IForgotPasswordService
{
    public Task ExecuteAsync(ForgotPasswordRequest request)
    {
        throw new Exception();
        // var user = await _authRepository.FindByEmailAsync(userForgotPasswordDto.Email);
        // if (user == null)
        //     throw new RestException(HttpStatusCode.BadRequest, new { Email = "There is no user with such email" });
        //
        // var token = await _authRepository.GeneratePasswordResetTokenAsync(user);
        //
        // var param = new Dictionary<string, string>
        // {
        //     {"token", token },
        //     {"email", user.Email }
        // };
        //
        // var callback = QueryHelpers.AddQueryString(userForgotPasswordDto.ClientURI, param);
        //
        // var emailBody = "Hi " + user.UserName + "<br/>Password change token<br/>" + "<a href=" + callback + ">Click here</a>";
        // _emailService.Send(
        //     "marque.repo@gmail.com",
        //     "Marque",
        //     "Change password",
        //     emailBody,
        //     user.Email);
    }
}