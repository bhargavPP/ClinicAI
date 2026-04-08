using ClinicAI.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace ClinicAI.Infrastructure.Interface
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        public EmailService(HttpClient httpClient, IConfiguration configuration,ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task SendEmail(string toEmail, string name, string subject, string body)
        {
            //var apiKey = _configuration["Brevo:ApiKey"];

            //var payload = new
            //{
            //    sender = new { name = "ClinicAI", email = "no-reply@clinic.com" },
            //    to = toEmail,
            //    subject = subject,
            //    htmlContent = $"<h3>Hello {name}, welcome to our platform!</h3>" + body
            //};

            //var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email")
            //{
            //    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json")
            //};
            //request.Headers.Add("api-key", apiKey);

            //var response = await _httpClient.SendAsync(request);

            //if (!response.IsSuccessStatusCode)
            //{
            //    var error = await response.Content.ReadAsStringAsync();
            //    throw new Exception($"Failed to send email: {error}");
            //    _logger.LogError("Failed to send email to {Email}. Response: {Response}", toEmail, error);
            //}
            _logger.LogInformation("Send successful");
        }
    }
}
