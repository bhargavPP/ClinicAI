using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Domain.Entities
{
    public class User
    {
        public Guid id {  get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public List<Patient> Patients { get; set; } = new();
        public string FullName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Optional (future)
        public bool IsEmailVerified { get; set; } = false;
    }
}
