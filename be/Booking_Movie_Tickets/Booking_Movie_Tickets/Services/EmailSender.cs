using Booking_Movie_Tickets.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Booking_Movie_Tickets.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Admin", _config["EmailSettings:SmtpUsername"]));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = message };

            using var client = new SmtpClient();
            await client.ConnectAsync(_config["EmailSettings:SmtpServer"],
                                      int.Parse(_config["EmailSettings:SmtpPort"]),
                                      SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_config["EmailSettings:SmtpUsername"],
                                           _config["EmailSettings:SmtpPassword"]);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }

    }

}
