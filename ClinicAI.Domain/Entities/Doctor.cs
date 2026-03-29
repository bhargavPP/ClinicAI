using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Domain.Entities 
{
    public class Doctor : BaseEntity
    {
       // public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public string Specialization { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<DoctorsAvailability> DoctorsAvailabilities { get; set; } = new List<DoctorsAvailability>();
    }
}
