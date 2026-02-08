using System.Net;
using System.Net.Mail;
using TodoListApp.Interfaces;

namespace TodoListApp.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration configuration;

    public EmailService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var from = this.configuration["EmailSettings:From"];
        var smtpServer = this.configuration["EmailSettings:SmtpServer"];
        var port = int.Parse(this.configuration["EmailSettings:SmtpPort"]!);
        var username = this.configuration["EmailSettings:Username"];
        var password = this.configuration["EmailSettings:Password"];

        using var message = new MailMessage(from!, toEmail, subject, body);

        message.IsBodyHtml = true;

        using var client = new SmtpClient(smtpServer, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true,
        };

        await client.SendMailAsync(message);
    }
}
