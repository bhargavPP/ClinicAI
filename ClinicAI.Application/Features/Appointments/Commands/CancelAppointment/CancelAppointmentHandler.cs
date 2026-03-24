using ClinicAI.Application.common.Models;
using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Appointments.Commands.CancelAppointment
{
    public class CancelAppointmentHandler : IRequestHandler<CancelAppointmentCommand, Result<bool>>
    {
        private readonly IClinicDbContext _context;

        public CancelAppointmentHandler(IClinicDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _context.Appointments
       .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (appointment == null)
                return Result<bool>.Failure("Appointment not found");

            var appointmentDateTime = appointment.AppointmentDate.Add(appointment.StartTime);

            if (appointmentDateTime < DateTime.Now)
                return Result<bool>.Failure("Cannot cancel past appointments");

            // Optional: prevent double cancel
            if (appointment.Status == "Cancelled")
                return Result<bool>.Failure("Cannot cancel past appointments");

            appointment.Status = "Cancelled";

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Failure("Appointment already cancelled");

        }
    }
}