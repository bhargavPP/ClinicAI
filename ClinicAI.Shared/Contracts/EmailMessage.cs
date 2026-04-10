using ClinicAI.Shared.Enums;

namespace ClinicAI.Shared.Contracts
{
    public class EmailMessage
    {
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public EmailType Type { get; set; }
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

        public int RetryCount { get; set; } = 0;    
    }
}
