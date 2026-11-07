namespace BuckleApp.Infrastructure.Interfaces;

public interface IEmailService
{
    Task<bool> Send(string mailSubject, string mailBodyText, string mailTo);
}