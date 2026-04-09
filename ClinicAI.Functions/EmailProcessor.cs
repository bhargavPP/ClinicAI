using Azure.Storage.Queues.Models;
using ClinicAI.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ClinicAI.Functions;

public class EmailProcessor
{
    private readonly ILogger<EmailProcessor> _logger;
    private readonly IEmailService _emailservice;
    public EmailProcessor(ILogger<EmailProcessor> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailservice = emailService; // ← inject it
    }

    [Function(nameof(EmailProcessor))]
    public async Task Run([QueueTrigger("email-queue", Connection = "AzureWebJobsStorage")] string message, FunctionContext context)
    {
        var logger = context.GetLogger("EmailProcessor");
        try
        {
           // var json = Encoding.UTF8.GetString(Convert.FromBase64String(message));
            var email = JsonSerializer.Deserialize<EmailMessage>(message);

            if (email != null)
            {
                await _emailservice.sendAsync(email.ToEmail, email.Subject, email.Body);
                logger.LogInformation($"Sending email to {email.ToEmail}");
            }
            else
            {
                logger.LogWarning("Received null email message");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing email");
            throw; // important for retry
        }
        _logger.LogInformation("C# Queue trigger function processed: {messageText}", message);
    }
}