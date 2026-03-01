using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("ScheduleX Admin",
            _config["EmailSettings:Email"]));

        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        message.Body = new TextPart("plain")
        {
            Text = body
        };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(
                _config["EmailSettings:Host"],
                int.Parse(_config["EmailSettings:Port"]),
                MailKit.Security.SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _config["EmailSettings:Email"],
                _config["EmailSettings:Password"]);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}