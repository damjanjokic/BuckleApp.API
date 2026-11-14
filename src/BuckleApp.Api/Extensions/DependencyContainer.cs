using BuckleApp.Application.Features.Authentication.RequestStudentLoginCode;
using BuckleApp.Application.Features.Authentication.VerifyStudentLoginCode;
using BuckleApp.Application.Features.Authentication.RefreshSession;
using BuckleApp.Application.Features.Authentication.LogoutUser;
using BuckleApp.Application.Features.Authentication.Services;
using BuckleApp.API.Requests.Auth;
using BuckleApp.Application.Features.Appointments;
using BuckleApp.Application.Features.Appointments.CancelAppointment;
using BuckleApp.Application.Features.Appointments.CreateAppointment;
using BuckleApp.Application.Features.Appointments.DeleteAppointment;
using BuckleApp.Application.Features.Appointments.GetAppointmentById;
using BuckleApp.Application.Features.Appointments.GetAppointmentsByOrganiserAndDate;
using BuckleApp.Application.Features.Appointments.PatchAppointment;
using BuckleApp.Application.Features.Authentication;
using BuckleApp.Application.Features.Authentication.ForgotPassword;
using BuckleApp.Application.Features.Authentication.LoginUser;
using BuckleApp.Application.Features.Authentication.ResetPassword;
using BuckleApp.Application.Features.Organisations;
using BuckleApp.Application.Features.Organisations.CreateOrganisation;
using BuckleApp.Application.Features.Organisations.GetOrganisationByName;
using BuckleApp.Application.Features.Users;
using BuckleApp.Application.Features.Users.GetUsersByOrgAndRole;
using BuckleApp.Core.IRepositories;
using BuckleApp.Core.IUnitOfWork;
using BuckleApp.Data.Repositories;
using BuckleApp.Data.UnitOfWork;
using BuckleApp.Infrastructure.Email;
using BuckleApp.Infrastructure.Interfaces;
using BuckleApp.Infrastructure.Security;

namespace BuckleApp.API.Extensions;

public static class DependencyContainer
{
    public static void ResolveDependenciesExtension(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
        builder.Services.AddScoped<IAuthRepository, AuthRepository>();
        builder.Services.AddScoped<IUserAccessor, UserAccessor>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IPasswordGenerator, PasswordGenerator>();
        builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IAppointmentsService, AppointmentsService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IOrganisationsService, OrganisationsService>();
        builder.Services.AddScoped<IUserService, UserService>();

        #region Appointments
        builder.Services.AddScoped<IGetAppointmentByIdService, GetAppointmentByIdService>();
        builder.Services.AddScoped<ICreateAppointmentsService, CreateAppointmentsService>();
        builder.Services.AddScoped<IPatchAppointmentService, PatchAppointmentService>();
        builder.Services.AddScoped<IGetAppointmentsByOrganiserAndDateService, GetAppointmentsByOrganiserAndDateService>();
        builder.Services.AddScoped<ICancelAppointmentService, CancelAppointmentService>();
        builder.Services.AddScoped<IDeleteAppointmentService, DeleteAppointmentService>();
        #endregion

        #region Auth

        builder.Services.AddScoped<ILoginCodeHasher, LoginCodeHasher>();
        builder.Services.AddScoped<IRequestStudentLoginCodeService, RequestStudentLoginCodeService>();
        builder.Services.AddScoped<IVerifyStudentLoginCodeService, VerifyStudentLoginCodeService>();
        builder.Services.AddScoped<IRefreshSessionService, RefreshSessionService>();
        builder.Services.AddScoped<ILogoutUserService, LogoutUserService>();
        builder.Services.AddScoped<IAuthenticationSessionService, AuthenticationSessionService>();


        builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
        builder.Services.AddScoped<ILoginUserService, LoginUserService>();
        builder.Services.AddScoped<IRegisterUserService, RegisterUserService>();
        builder.Services.AddScoped<IResetPasswordService, ResetPasswordService>();
        
        #endregion

        #region Organisation

        builder.Services.AddScoped<ICreateOrganisationService, CreateOrganisationService>();
        builder.Services.AddScoped<IGetOrganisationByNameService, GetOrganisationByNameService>();
        
        #endregion

        #region Users

        builder.Services.AddScoped<IGetUsersByOrgAndRoleService, GetUsersByOrgAndRoleService>();

        #endregion
    }
}