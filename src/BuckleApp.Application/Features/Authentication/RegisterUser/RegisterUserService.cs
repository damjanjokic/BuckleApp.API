using System.Net;
using System.Text;
using BuckleApp.Application.Exceptions;
using BuckleApp.Application.Features.Authentication.RegisterUser;
using BuckleApp.Core.Entities;
using BuckleApp.Core.Enumerations;
using BuckleApp.Core.IRepositories;
using BuckleApp.Application.Features.Authentication.RequestStudentLoginCode;
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
    private readonly IRequestStudentLoginCodeService _requestStudentLoginCode;

    public RegisterUserService(IAuthRepository authRepository, IEmailService emailService, IPasswordGenerator passwordGenerator, IUserAccessor userAccessor, IRequestStudentLoginCodeService requestStudentLoginCode)
    {
        _authRepository = authRepository;
        _emailService = emailService;
        _passwordGenerator = passwordGenerator;
        _userAccessor = userAccessor;
        _requestStudentLoginCode = requestStudentLoginCode;
    }

    public async Task<Guid> ExecuteAsync(RegisterUserRequest request)
    {
        var actor = await _authRepository.FindByIdAsync(_userAccessor.GetCurrentUserId());
        if (actor == null || actor.isDeleted || actor.IsDisabled || actor.LockoutEnd > DateTimeOffset.UtcNow)
            throw new RestException(HttpStatusCode.Forbidden);
        var roles = (await _authRepository.GetUserRoles(actor)).ToList();
        var permitted = request.Role != Role.SuperAdmin && Enum.IsDefined(request.Role) &&
            (roles.Contains("SuperAdmin") ||
             (actor.OrganisationId == request.OrganisationId &&
              ((roles.Contains("Admin") && request.Role is Role.Instructor or Role.Student) ||
               (roles.Contains("Instructor") && request.Role == Role.Student))));
        if (!permitted || !await _authRepository.OrganisationExistsAsync(request.OrganisationId))
            throw new RestException(HttpStatusCode.Forbidden);
        if (await _authRepository.UserExistsQueryAsync(x => x.NormalizedEmail == request.Email.ToUpper()))
            throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already exists" });

        var user = request.Adapt<User>();
        if (roles.Contains("Instructor") && request.Role == Role.Student) user.InstructorId = actor.Id;
        var result = await _authRepository.CreateWithRoleAsync(user,
            request.Role == Role.Student ? null : _passwordGenerator.GenerateRandomPassword(), request.Role);
        if (!result.Succeeded)
            throw new RestException(HttpStatusCode.BadRequest, "Problem creating user");

        if (request.Role != Role.Student)
        {
            var token = await _authRepository.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            
            var emailResult = await _emailService.Send("Account confirmation", "Hello, it's me " + token, user.Email);

            if (!emailResult)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Reset mail was not sent");
            }
            
        }

        if (request.Role == Role.Student)
            await _requestStudentLoginCode.ExecuteAsync(new RequestStudentLoginCodeRequest { Email = user.Email! });

        return user.Id;
    }
}