using ClinicAI.Domain.Entities;

namespace ClinicAI.Application.Features.Availability.Queries.GetAvailability
{
    public class AvailabilityDto
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public string DoctorName { get; set; } = null!;
    }
}
