using ClinicAI.Application.common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Doctors.Commands.CreateDoctor
{
    public class CreateDoctorCommand :IRequest<Result<Guid>>
    {
        public string Name { get; set; } = null!;
        public string Specialization { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
