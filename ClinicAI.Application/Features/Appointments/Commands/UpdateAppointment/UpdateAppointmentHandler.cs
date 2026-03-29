using ClinicAI.Application.common.Models;
using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Appointments.Commands.UpdateAppointment
{
    public class UpdateAppointmentHandler : BaseHandler, IRequestHandler<UpdateAppointmentCommand, Result<bool>>
    {
    
        public UpdateAppointmentHandler(IClinicDbContext context ,ICurrentUserService currentUser, IDateTime dateTime)
            : base(context, currentUser, dateTime)
        {
        }
        public async Task<Result<bool>> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == request.Id && a.Patient.UserId==currentUserId, cancellationToken);

            if (appointment == null)
                return Result<bool>.Failure("Appointment not found");

            if (appointment.Status == "Cancelled")
                return Result<bool>.Failure("Cannot update cancelled appointment");

            if (appointment.AppointmentDate < _dateTime.dateTimeUtcNow)
                return Result<bool>.Failure("Cannot update past appointments");

            var endTime = request.StartTime.Add(TimeSpan.FromMinutes(20));

            // 🔥 conflict check
            var conflict = await _context.Appointments
                .Where(a => a.DoctorId == request.DoctorId &&
                            a.AppointmentDate.Date == request.AppointmentDate.Date &&
                            a.Id != request.Id)
                .AnyAsync(a => request.StartTime < a.EndTime && endTime > a.StartTime, cancellationToken);

            if (conflict)
                return Result<bool>.Failure("Time slot already booked");

            // ✅ update
            appointment.DoctorId = request.DoctorId;
            appointment.AppointmentDate = request.AppointmentDate;
            appointment.StartTime = request.StartTime;
            appointment.EndTime = endTime;
            appointment.Notes = request.Notes;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "Appointment updated successfully");
        }
    }
}
