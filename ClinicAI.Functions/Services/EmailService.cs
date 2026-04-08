using System.Net;
using System.Net.Mail;

namespace ClinicAI.Functions.Services
{
    public class EmailService : IEmailService
    {
        public async Task sendAsync(string to, string subject, string body)
        {
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("bhargavpatel4319@gmail.com", "gnfsrklkdvxrveas"),
                EnableSsl = true
            };

            var mail = new MailMessage("bhargavpatel4319@gmail.com", to, subject, body);
            await client.SendMailAsync(mail);
        }
    }
}
