using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Doctors.Commands.DeleteDoctor
{
    public class DeleteDoctorValidator:AbstractValidator<DeleteDoctorCommand>
    {
        public DeleteDoctorValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
