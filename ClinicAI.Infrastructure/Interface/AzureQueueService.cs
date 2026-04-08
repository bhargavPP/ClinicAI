using Azure.Storage.Queues;
using ClinicAI.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ClinicAI.Infrastructure.Interface
{
    public class AzureQueueService : IQueueService
    {
        private readonly QueueClient _queueClient;
        public AzureQueueService(IConfiguration configuration)
        {
            var connectingString = configuration["AzureQueue:ConnectionString"];
            var queueName = configuration["AzureQueue:QueueName"];

            _queueClient = new QueueClient(connectingString, queueName);
            _queueClient.CreateIfNotExists();
        }
        public async Task EnqueuAsync<T>(T message)
        {
            var json = JsonSerializer.Serialize(message);
            var base64Message = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));

            await _queueClient.SendMessageAsync(base64Message);
        }
    }
}
