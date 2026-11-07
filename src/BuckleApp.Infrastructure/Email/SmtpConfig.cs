namespace BuckleApp.Infrastructure.Email;

public class SmtpConfig
{
    public string ApiKey { get; set; }

    public string SenderEmail { get; set; }

    public string SenderName { get; set; }
    // public string Server { get; set; }
    // public int Port { get; set; }
    // public bool EmailEnableSSL { get; set; }
    // public string EmailSmtpUsername { get; set; }
    // public string EmailSmtpPassword { get; set; }
}