namespace ClinicAI.Application.Interfaces
{
    public interface IQueueService
    {
        Task EnqueuAsync<T>(T message);
    }
}
