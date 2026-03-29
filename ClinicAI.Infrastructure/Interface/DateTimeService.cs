using ClinicAI.Application.Interfaces;

namespace ClinicAI.Infrastructure.Interface
{
    public class DateTimeService :IDateTime
    {
        public DateTime dateTimeUtcNow => DateTime.UtcNow;
    }
}
