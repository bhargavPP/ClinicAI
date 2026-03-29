    namespace ClinicAI.Domain.Entities
{
    public class User : BaseEntity
    {
     //   public Guid id {  get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public List<Patient> Patients { get; set; } = new();
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;   // ← add
        public string Role { get; set; } = "Patient";      // ← add: Patient | Doctor | Admin
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Optional (future)
        public bool IsEmailVerified { get; set; } = false;
        // ← add refresh token navigation
        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }
}
