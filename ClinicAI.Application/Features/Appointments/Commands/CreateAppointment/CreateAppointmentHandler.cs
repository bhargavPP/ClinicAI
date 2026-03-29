using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClinicAI.Application.Constants;
using ClinicAI.Application.common.Models;
namespace ClinicAI.Application.Features.Appointments.Commands.CreateAppointment
{
    public class CreateAppointmentHandler : BaseHandler, IRequestHandler<CreateAppointmentCommand, Result<Guid>>
    {

        public CreateAppointmentHandler(IClinicDbContext context, ICurrentUserService currentUser, IDateTime dateTime)
            : base(context, currentUser, dateTime)
        {
        }
        public async Task<Result<Guid>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {

            var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == request.DoctorId, cancellationToken);

            if (!doctorExists)
                return Result<Guid>.Failure("Doctor not found");

            var patientExists = await _context.Patients.AnyAsync(p => p.Id == request.PatientId, cancellationToken);

            if (!patientExists)
                return Result<Guid>.Failure("Patient not found");

            var patient = await _context.Patients
    .FirstOrDefaultAsync(p => p.Id == request.PatientId
                           && p.UserId == _currentUser.UserId);

            if (!patientExists)
                return Result<Guid>.Failure("Invalid Patient");

            // ✅ Compute EndTime internally
            var endTime = request.StartTime.Add(
                TimeSpan.FromMinutes(Constant.SlotDurationMinutes));


            var hasConflict = await _context.Appointments
                                    .Where(a => a.DoctorId == request.DoctorId &&
                                    a.AppointmentDate == request.AppointmentDate)
                                    .AnyAsync(a => (request.StartTime < a.EndTime && endTime > a.StartTime), cancellationToken);
            if (hasConflict)
                return Result<Guid>.Failure("Time Slot Not Available.");

            var availability = await _context.DoctorsAvailabilities
                                            .FirstOrDefaultAsync(a =>
                                                a.DoctorId == request.DoctorId &&
                                                a.Date.Date == request.AppointmentDate.Date &&
                                                a.IsAvailable,
                                                cancellationToken);

            if (availability == null)
                return Result<Guid>.Failure("Doctor not available");

            if (request.StartTime < availability.StartTime || endTime > availability.EndTime)
                return Result<Guid>.Failure("Outside availability");
            var appointment = new Domain.Entities.Appointment
            {
                Id = Guid.NewGuid(),
                DoctorId = request.DoctorId,
                PatientId = request.PatientId,
                AppointmentDate = request.AppointmentDate,
                StartTime = request.StartTime,
                EndTime = endTime,
                Notes = request.Notes,
                Status = "Scheduled"
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(
              appointment.Id,
              "Appointment booked successfully"
          );
        }
    }

}
