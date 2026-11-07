using System.Net;
using BuckleApp.Application.Exceptions;
using BuckleApp.Core.Enumerations;
using BuckleApp.Core.IRepositories;
using BuckleApp.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BuckleApp.Application.Features.Authentication.LoginUser;

public class LoginUserService : ILoginUserService
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtGenerator _jwtGenerator;

    public LoginUserService(IJwtGenerator jwtGenerator, IAuthRepository authRepository)
    {
        _jwtGenerator = jwtGenerator;
        _authRepository = authRepository;
    }
    
    public async Task<string> ExecuteAsync(LoginUserRequest request)
    {
        var result = false;
        var user = await _authRepository.FindByEmailAsync(request.Email);

        if (user == null)
            throw new RestException(HttpStatusCode.Unauthorized);

        /*if (!user.EmailConfirmed)
            throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email is not confirmed" });*/
        var roles = await _authRepository.GetUserRoles(user);
        if (roles!.First() == Role.Student.ToString())
        {
            var claims = await _authRepository.GetClaimsAsync(user);
            var loginCodeClaim = claims.FirstOrDefault(c => c.Type == "OneTimeLoginCode");
            if (loginCodeClaim == null || loginCodeClaim.Value != request.Password)
                throw new RestException(HttpStatusCode.BadRequest, "Invalid or expired code");
            await _authRepository.RemoveClaimAsync(user, loginCodeClaim);
            result = true;
        }
        else
        {
            result = await _authRepository
                .CheckPasswordSignInAsync(user, request.Password, false);
        }

        if (result)
        {
            return _jwtGenerator.CreateToken(user, roles.ToList());
        }

        throw new RestException(HttpStatusCode.Unauthorized);
    }
}