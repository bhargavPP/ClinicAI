namespace ClinicAI.Domain.Entities
{
    public class DoctorsAvailability
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public Doctor Doctor { get; set; } = null!;
    }
}
