using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Doctors.Commands.UpdateDoctor
{
    public class UpdateDoctorCommand:IRequest<bool>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string Specialization { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
