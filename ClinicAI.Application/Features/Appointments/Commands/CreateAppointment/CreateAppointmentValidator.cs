using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Appointments.Commands.CreateAppointment
{
    public class CreateAppointmentValidator :AbstractValidator<CreateAppointmentCommand>
    {
        public CreateAppointmentValidator()
        {
            RuleFor(x=>x.DoctorId).NotEmpty().WithMessage("Doctor is required.");
            RuleFor(x=>x.PatientId).NotEmpty().WithMessage("Patient is required.");

            RuleFor(x=>x.AppointmentDate)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("Appointment date must be today or in the future.");

        }
    }
}
