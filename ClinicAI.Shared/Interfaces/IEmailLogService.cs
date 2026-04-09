namespace ClinicAI.Shared.Interfaces
{
    public interface IEmailLogService
    {
        Task<Guid> CreateAsync(string toEmail, Guid? userId, string subject, string body, string correlationId);
        Task MarkSentAsync(Guid id);
        Task MarkFailedAsync(Guid id, string error);
        Task IncrementRetryAsync(Guid id);
    }
}
