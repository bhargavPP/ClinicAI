namespace ClinicAI.Functions.Services
{
    public interface IEmailService
    {
        Task sendAsync(string to,string subject, string body);
    }
}
