using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Appointments.Commands.UpdateAppointment
{
    public class UpdateAppointmentValidator:AbstractValidator<UpdateAppointmentCommand>
    {
        public UpdateAppointmentValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Appointment Id is required.");

            RuleFor(x => x.DoctorId)
                .NotEmpty()
                .WithMessage("Doctor is required.");
            RuleFor(x => x.PatientId)
               .NotEmpty()
               .WithMessage("Patient is required.");
            RuleFor(x => x.AppointmentDate)
                .NotEmpty()
                .WithMessage("Appointment date is required.")
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("Appointment date must be today or future.");

            RuleFor(x => x.StartTime)
                .NotEmpty()
                .WithMessage("Start time is required.")
                .Must((model, startTime) =>
                {
                    var appointmentDateTime = model.AppointmentDate.Date + startTime;
                    return appointmentDateTime >= DateTime.Now;
                })
                .WithMessage("Appointment time must be in the future.");

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
