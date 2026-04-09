using ClinicAI.Domain.Entities;

using ClinicAI.Infrastructure.Persistence;
using ClinicAI.Shared.Interfaces;
namespace ClinicAI.Infrastructure.Interface
{
    public class EmailLogService : IEmailLogService
    {
        private readonly ClinicDbContext _context;

        public EmailLogService(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateAsync(string toEmail, Guid? userId, string subject, string body, string correlationId)
        {
            var log = new EmailLog
            {
                UserId = userId,
                ToEmail = toEmail,
                Subject = subject,
                Body = body,
                CorrelationId = correlationId,
                Status = EmailStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _context.EmailLogs.AddAsync(log);
            await _context.SaveChangesAsync();
            return log.Id;
        }

        public async Task MarkSentAsync(Guid id)
        {
            var log = await _context.EmailLogs.FindAsync(id);
            if (log == null) return;

            log.Status = EmailStatus.Sent;
            log.SentAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task MarkFailedAsync(Guid id, string error)
        {
            var log = await _context.EmailLogs.FindAsync(id);
            if (log == null) return;

            log.Status = EmailStatus.Failed;
            log.ErrorMessage = error;
            log.FailedAt = DateTime.UtcNow;
            log.RetryCount++;
            await _context.SaveChangesAsync();
        }

        public async Task IncrementRetryAsync(Guid id)
        {
            var log = await _context.EmailLogs.FindAsync(id);
            if (log == null) return;

            log.Status = EmailStatus.Retrying;
            log.RetryCount++;
            await _context.SaveChangesAsync();
        }
    }
}