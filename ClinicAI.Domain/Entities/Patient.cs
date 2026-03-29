namespace ClinicAI.Domain.Entities
{
    public class Patient:BaseEntity
    {
     //   public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Email { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public string? RelationshipToUser { get; set; } = "Self";
        public Guid? UserId { get; set; } 
        public User User { get; set; } = null!;
    }
}
