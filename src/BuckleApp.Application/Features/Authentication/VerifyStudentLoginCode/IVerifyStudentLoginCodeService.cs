using BuckleApp.Application.Dtos;

namespace BuckleApp.Application.Features.Authentication.VerifyStudentLoginCode;

public interface IVerifyStudentLoginCodeService
{
    Task<AuthenticationResponse> ExecuteAsync(VerifyStudentLoginCodeRequest request);
}
