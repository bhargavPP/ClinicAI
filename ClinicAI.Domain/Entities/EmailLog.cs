namespace ClinicAI.Domain.Entities
{
    public class EmailLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // User tracking
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public string ToEmail { get; set; } = string.Empty;

        // Email details
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public EmailType Type { get; set; }  // Registration, Appointment, Password Reset etc
        public EmailStatus Status { get; set; } = EmailStatus.Pending;

        // Tracking
        public string? ErrorMessage { get; set; }
        public int RetryCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SentAt { get; set; }
        public DateTime? FailedAt { get; set; }

        // Metadata
        public string? MessageId { get; set; }  // SMTP message ID
        public string? CorrelationId { get; set; }  // trace across services
    }

    public enum EmailStatus
    {
        Pending,
        Sent,
        Failed,
        Retrying
    }

    public enum EmailType
    {
        Registration,
        AppointmentConfirmation,
        AppointmentCancellation,
        AppointmentReminder,
        PasswordReset,
        General
    }
}
