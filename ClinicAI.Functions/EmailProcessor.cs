
using Azure.Storage.Queues;
using ClinicAI.Shared;
using ClinicAI.Shared.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ClinicAI.Functions;

public class EmailProcessor
{
    private readonly ILogger<EmailProcessor> _logger;
    private readonly IEmailService _emailService;
    private readonly ClinicAI.Shared.Interfaces.IEmailLogService _emailLogService;
    private readonly QueueClient _queueClient;
    public EmailProcessor(
        ILogger<EmailProcessor> logger,
        IEmailService emailService,
         ClinicAI.Shared.Interfaces.IEmailLogService emailLogService, IConfiguration config)
    {
        _logger = logger;
        _emailService = emailService;
        _emailLogService = emailLogService;
        _queueClient = new QueueClient(
            config["AzureWebJobsStorage"],
            "email-queue"
        );
    }

    [Function(nameof(EmailProcessor))]
    public async Task Run(
        [QueueTrigger("email-queue", Connection = "AzureWebJobsStorage")] string message,
        FunctionContext context)
    {
        var correlationId = string.Empty;
         
        var email = JsonSerializer.Deserialize<EmailMessage>(message)
      ?? throw new InvalidOperationException("Invalid message");

        var logId = Guid.NewGuid();
        try
        {
             correlationId = email.CorrelationId;

            _logger.LogInformation(" Processing | CorrelationId: {CorrelationId} | To: {To}", correlationId, email.ToEmail);

            // Create pending log
            logId = await _emailLogService.CreateAsync(email.ToEmail,email.UserId,email.Subject,email.Body,correlationId);

            // Send email
            await _emailService.SendEmailAsync(email.ToEmail,email.UserId.ToString(), email.Subject, email.Body);

            // Mark sent
            await _emailLogService.MarkSentAsync(logId);

            _logger.LogInformation("Email sent | CorrelationId: {CorrelationId} | To: {To}", correlationId, email.ToEmail);
        }
        catch (Exception ex)
        {
            email.RetryCount++;

            _logger.LogError(ex, "Failed | CorrelationId: {CorrelationId}",correlationId);

            if (email.RetryCount >= 3)
            {
                // Final failure (NO THROW)
                await _emailLogService.MarkFailedAsync(logId, ex.Message);
                return;
            }

            await RequeueMessage(email);
        }
    }

    private async Task RequeueMessage(EmailMessage email)
    {
        var json = JsonSerializer.Serialize(email);
        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

        await _queueClient.SendMessageAsync(
            base64,
            visibilityTimeout: TimeSpan.FromSeconds(30)
        );
    }
}