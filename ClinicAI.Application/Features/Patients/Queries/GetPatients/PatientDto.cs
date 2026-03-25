namespace ClinicAI.Application.Features.Patients.Queries.GetPatients
{
    public class PatientDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
