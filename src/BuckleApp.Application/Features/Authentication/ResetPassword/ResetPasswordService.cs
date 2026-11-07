using System.Net;
using System.Text;
using BuckleApp.Application.Exceptions;
using BuckleApp.Core.IRepositories;
using Microsoft.AspNetCore.WebUtilities;

namespace BuckleApp.Application.Features.Authentication.ResetPassword;

public class ResetPasswordService : IResetPasswordService
{
    private readonly IAuthRepository _authRepository;

    public ResetPasswordService(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<bool> ExecuteAsync(ResetPasswordRequest request)
    {
        var user = await _authRepository.FindByEmailAsync(request.Email);

        if (user == null)
            throw new RestException(HttpStatusCode.NotFound, "There is no user with this email address");
        
        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

        var reset = await _authRepository.ResetPasswordAsync(user, decodedToken, request.Password);
        if (reset.Errors.Any())
        {
            throw new RestException(HttpStatusCode.BadRequest, reset.Errors);
        }

        if (!user.EmailConfirmed)
        {
            user.EmailConfirmed = true;
            await _authRepository.UpdateAsync(user);
        }

        return true;
    }
}