using LearnAPI.Modal;
using LearnAPI.Service;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;

namespace LearnAPI.Container
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;
        public EmailService(IOptions<EmailSettings> options)
        {
            this.emailSettings = options.Value;
        }

        public async Task SendEmail(Mailrequest mailrequest)
        {
            try
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(emailSettings.Email);
                email.To.Add(MailboxAddress.Parse(mailrequest.Email));
                email.Subject = mailrequest.Subject;
                var builder = new BodyBuilder();
                builder.HtmlBody = mailrequest.Emailbody;
                email.Body = builder.ToMessageBody();

                using var smptp = new SmtpClient();
               /* await smptp.ConnectAsync(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
                await smptp.AuthenticateAsync(emailSettings.Email, emailSettings.Password);
                await smptp.SendAsync(email);
                await smptp.DisconnectAsync(true);*/
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw; // Re-throw the exception or handle as needed
            }
        }
    }
}

