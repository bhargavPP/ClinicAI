using ClinicAI.Application.common.Models;
using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Appointments.Commands.CancelAppointment
{
    public class CancelAppointmentHandler :BaseHandler, IRequestHandler<CancelAppointmentCommand, Result<bool>>
    {
        public CancelAppointmentHandler(IClinicDbContext context, ICurrentUserService currentUser, IDateTime dateTime)
            : base(context, currentUser, dateTime)
        {
        }

        public async Task<Result<bool>> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _context.Appointments
       .FirstOrDefaultAsync(a => a.Id == request.Id && a.PatientId == currentUserId, cancellationToken);

            if (appointment == null)
                return Result<bool>.Failure("Appointment not found");

            var appointmentDateTime = appointment.AppointmentDate.Add(appointment.StartTime);

            if (appointmentDateTime < _dateTime.dateTimeUtcNow)
                return Result<bool>.Failure("Cannot cancel past appointments");

            // Optional: prevent double cancel
            if (appointment.Status == "Cancelled")
                return Result<bool>.Failure("Cannot cancel past appointments");

            appointment.Status = "Cancelled";
            
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);

        }
    }
}