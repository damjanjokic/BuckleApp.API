using BuckleApp.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BuckleApp.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly SmtpConfig _config;

    public EmailService(IOptions<SmtpConfig> config)
    {
        _config = config.Value;
    }
    
    public async Task<bool> Send(string mailSubject, string mailBodyText, string mailTo)
    {
        var client = new SendGridClient(_config.ApiKey);
        var from = new EmailAddress(_config.SenderEmail, _config.SenderName);
        var to = new EmailAddress(mailTo);
        var subject = mailSubject;
        var htmlContent = mailBodyText;
        var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
        var response = await client.SendEmailAsync(msg);

        return response.IsSuccessStatusCode;
    }

    // public Task Send(string from, string senderName, string subject, string bodyText, string to)
    // {
    //     var message = CreateMailMessage(from, senderName, subject, bodyText, to);
    //     var client = CreateSmtpClient();
    //
    //     client.SendCompleted += (_, e) =>
    //     {
    //         //need to implement logger
    //         client.Dispose();
    //         message.Dispose();
    //     };
    //
    //     return client.SendMailAsync(message);
    // }
    //
    // private MailMessage CreateMailMessage(string from, string senderName, string subject, string bodyText, string to)
    // {
    //     MailAddress fromMail = new MailAddress(from, senderName);
    //     MailAddress toMail = new MailAddress(to);
    //
    //     return new MailMessage(fromMail, toMail)
    //     {
    //         IsBodyHtml = true,
    //         Body = bodyText,
    //         BodyEncoding = Encoding.UTF8,
    //         Subject = subject,
    //         SubjectEncoding = Encoding.UTF8,
    //     };
    // }

    // private SmtpClient CreateSmtpClient()
    // {
    //     var credentials = new NetworkCredential(_config.EmailSmtpUsername, _config.EmailSmtpPassword);
    //     var client = new SmtpClient(_config.Server, _config.Port)
    //     {
    //         DeliveryMethod = SmtpDeliveryMethod.Network,
    //         EnableSsl = _config.EmailEnableSSL,
    //         Credentials = credentials
    //     };
    //
    //     return client;
    // }

}