using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Doctors.Queries.GetDoctors
{
    public class DoctorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Specialization { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
