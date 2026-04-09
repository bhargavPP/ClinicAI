namespace ClinicAI.Shared
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string name, string subject, string body);
    }
}
