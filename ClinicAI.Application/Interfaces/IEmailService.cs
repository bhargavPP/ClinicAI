namespace ClinicAI.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(string toEmail, string name, string subject, string body);
    }
}
