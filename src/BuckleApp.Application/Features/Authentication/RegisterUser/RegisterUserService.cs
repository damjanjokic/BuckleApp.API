using System.Net;
using System.Security.Claims;
using System.Text;
using BuckleApp.Application.Exceptions;
using BuckleApp.Application.Features.Authentication.RegisterUser;
using BuckleApp.Core.Entities;
using BuckleApp.Core.Enumerations;
using BuckleApp.Core.IRepositories;
using BuckleApp.Core.IUnitOfWork;
using BuckleApp.Infrastructure.Interfaces;
using Mapster;
using Microsoft.AspNetCore.WebUtilities;

namespace BuckleApp.API.Requests.Auth;

public class RegisterUserService : IRegisterUserService
{
    private readonly IAuthRepository _authRepository;
    private readonly IEmailService _emailService;
    private readonly IPasswordGenerator _passwordGenerator;
    private readonly IUserAccessor _userAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserService(IAuthRepository authRepository, IEmailService emailService, IPasswordGenerator passwordGenerator, IUserAccessor userAccessor, IUnitOfWork unitOfWork)
    {
        _authRepository = authRepository;
        _emailService = emailService;
        _passwordGenerator = passwordGenerator;
        _userAccessor = userAccessor;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> ExecuteAsync(RegisterUserRequest request)
    {
     // var loggedInUser = await _unitOfWork.User.GetById(_userAccessor.GetCurrentUserId());
        // var loggedInUserRoles = await _authRepository.GetUserRoles(loggedInUser);
        if (await _authRepository.UserExistsQueryAsync(x => x.NormalizedEmail == request.Email.ToUpper()))
            throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already exists" });

        var user = request.Adapt<User>();
        var result = await _authRepository.CreateAsync(user, _passwordGenerator.GenerateRandomPassword());

        if (!result.Succeeded)
            throw new RestException(HttpStatusCode.BadRequest, new { Email = "Problem creating user" });
        
        var roleAdded = await _authRepository.AddToRoleAsync(user, request.Role);
        if (!roleAdded)
            throw new RestException(HttpStatusCode.BadRequest, "User role was not saved");
        
        if (request.Role != Role.Student)
        {
            // if (!loggedInUserRoles.Contains(Role.SuperAdmin.ToString()) ||
            //     !loggedInUserRoles.Contains(Role.Admin.ToString()))
            // {
            //     throw new RestException(HttpStatusCode.Forbidden, "You do not have permission to create student");
            // }
            
            var token = await _authRepository.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            
            var emailResult = await _emailService.Send("Account confirmation", "Hello, it's me " + token, user.Email);

            if (!emailResult)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Reset mail was not sent");
            }
            
        }

        if (request.Role == Role.Student)
        {
            // if (!loggedInUserRoles.Contains(Role.Admin.ToString()) ||
            //     !loggedInUserRoles.Contains(Role.Instructor.ToString()))
            // {
            //     throw new RestException(HttpStatusCode.Forbidden, "You do not have permission to create student");
            // }
            var claims = await _authRepository.GetClaimsAsync(user);
            var existingClaim = claims.FirstOrDefault(c => c.Type == "OneTimeLoginCode");
            if (existingClaim != null)
                await _authRepository.RemoveClaimAsync(user, existingClaim);
            
            var loginCode = new Random().Next(100000, 999999).ToString();

            var addClaimResult = await _authRepository.AddClaimAsync(user, new Claim("OneTimeLoginCode", loginCode));
            
            if (!addClaimResult.Succeeded)
                throw new RestException(HttpStatusCode.BadRequest, "Failed adding one-time code for login");

            var emailResult = await _emailService.Send("ExecuteAsync code", loginCode, request.Email);
            
            if (!emailResult)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Reset mail was not sent");
            }
        }

        return user.Id;
    }
}