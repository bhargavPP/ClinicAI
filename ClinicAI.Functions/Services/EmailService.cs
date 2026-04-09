using System.Net;
using System.Net.Mail;

namespace ClinicAI.Functions.Services
{
    public class EmailService : IEmailService
    {
        public async Task sendAsync(string to, string subject, string body)
        {
            var host = Environment.GetEnvironmentVariable("EmailHost");
            var port = int.Parse(Environment.GetEnvironmentVariable("EmailPort"));
            var username = Environment.GetEnvironmentVariable("EmailUsername");
            var password = Environment.GetEnvironmentVariable("EmailPassword");

            var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            var mail = new MailMessage(username, to, subject, body);
            await client.SendMailAsync(mail);
         }
    }
}
