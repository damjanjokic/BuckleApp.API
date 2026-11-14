namespace BuckleApp.Application.Features.Authentication.RequestStudentLoginCode;

public interface IRequestStudentLoginCodeService
{
    Task ExecuteAsync(RequestStudentLoginCodeRequest request);
}
