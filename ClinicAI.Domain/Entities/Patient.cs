using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Domain.Entities
{
    public class Patient
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Email { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public string? RelationshipToUser { get; set; } = null;
        public Guid? UserId { get; set; } 
        public User User { get; set; } = null!;
    }
}
