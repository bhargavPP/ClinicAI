using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Doctors.Commands.DeleteDoctor
{
    public class DeleteDoctorCommand:IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
