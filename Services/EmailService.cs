using System.Net;
using System.Net.Mail;

namespace CampusFlow.Mvc.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(
        string toEmail,
        string subject,
        string body)
    {
        string? host =
            _configuration["EmailSettings:Host"];

        int port =
            int.Parse(_configuration["EmailSettings:Port"]!);

        string? username =
            _configuration["EmailSettings:Username"];

        string? password =
            _configuration["EmailSettings:Password"];

        using var client = new SmtpClient(host, port);

        client.Credentials =
            new NetworkCredential(username, password);

        client.EnableSsl = true;

        using var message = new MailMessage();

        message.From = new MailAddress(username!);

        message.To.Add(toEmail);

        message.Subject = subject;

        message.Body = body;

        message.IsBodyHtml = true;

        await client.SendMailAsync(message);
    }
}