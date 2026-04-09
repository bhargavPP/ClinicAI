
using ClinicAI.Shared;
using ClinicAI.Shared.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ClinicAI.Functions;

public class EmailProcessor
{
    private readonly ILogger<EmailProcessor> _logger;
    private readonly IEmailService _emailService;
    private readonly ClinicAI.Shared.Interfaces.IEmailLogService _emailLogService;

    public EmailProcessor(
        ILogger<EmailProcessor> logger,
        IEmailService emailService,
         ClinicAI.Shared.Interfaces.IEmailLogService emailLogService)
    {
        _logger = logger;
        _emailService = emailService;
        _emailLogService = emailLogService;
    }

    [Function(nameof(EmailProcessor))]
    public async Task Run(
        [QueueTrigger("email-queue", Connection = "AzureWebJobsStorage")] string message,
        FunctionContext context)
    {
        var correlationId = string.Empty;
        Guid logId = Guid.Empty;

        try
        {
            var email = JsonSerializer.Deserialize<EmailMessage>(message)
                ?? throw new InvalidOperationException("Failed to deserialize message");

            correlationId = email.CorrelationId;

            _logger.LogInformation(
                "📧 Processing | CorrelationId: {CorrelationId} | To: {To}",
                correlationId, email.ToEmail);

            // Create pending log
            logId = await _emailLogService.CreateAsync(email.ToEmail,email.UserId,email.Subject,email.Body,correlationId);

            // Send email
            await _emailService.SendEmailAsync(email.ToEmail,email.UserId.ToString(), email.Subject, email.Body);

            // Mark sent
            await _emailLogService.MarkSentAsync(logId);

            _logger.LogInformation(
                "✅ Email sent | CorrelationId: {CorrelationId} | To: {To}",
                correlationId, email.ToEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ Failed | CorrelationId: {CorrelationId}",
                correlationId);

            if (logId != Guid.Empty)
                await _emailLogService.MarkFailedAsync(logId, ex.Message);

            throw; // retry
        }
    }
}