using ClinicAI.Application.Interfaces;
using ClinicAI.Domain.Entities;

namespace ClinicAI.Functions
{
    public interface IEmailLogService
    {
        Task<Guid> CreateAsync(EmailLog log);
        Task MarkSentAsync(Guid id);
        Task MarkFailedAsync(Guid id, string error);
        Task IncrementRetryAsync(Guid id);
    }

    public class EmailLogService : IEmailLogService
    {
        private readonly IClinicDbContext _context;

        public EmailLogService(IClinicDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateAsync(EmailLog log)
        {
            await _context.EmailLogs.AddAsync(log);
            await _context.SaveChangesAsync(CancellationToken.None);
            return log.Id;
        }

        public async Task MarkSentAsync(Guid id)
        {
            var log = await _context.EmailLogs.FindAsync(id);
            if (log == null) return;

            log.Status = EmailStatus.Sent;
            log.SentAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(CancellationToken.None);
        }

        public async Task MarkFailedAsync(Guid id, string error)
        {
            var log = await _context.EmailLogs.FindAsync(id);
            if (log == null) return;

            log.Status = EmailStatus.Failed;
            log.ErrorMessage = error;
            log.FailedAt = DateTime.UtcNow;
            log.RetryCount++;
            await _context.SaveChangesAsync(CancellationToken.None);
        }

        public async Task IncrementRetryAsync(Guid id)
        {
            var log = await _context.EmailLogs.FindAsync(id);
            if (log == null) return;

            log.Status = EmailStatus.Retrying;
            log.RetryCount++;
            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}
