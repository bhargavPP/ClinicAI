using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace ClinicAI.Application.Features.Appointments.Commands.CreateAppointment
{
    public class CreateAppointmentHandler : IRequestHandler<CreateAppointmentCommand, Guid>
    {
        private readonly IClinicDbContext _context;
        public CreateAppointmentHandler(IClinicDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == request.DoctorId, cancellationToken);

            if (!doctorExists)
                throw new Exception("Doctor not found");

            var patientExists =await _context.Patients.AnyAsync(p => p.Id == request.PatientId, cancellationToken);

            if(!patientExists)
                throw new Exception("Patient not found");

            var hasConflict = await _context.Appointments
                                    .Where(a=> a.DoctorId == request.DoctorId && 
                                    a.AppointmentDate == request.AppointmentDate)
                                    .AnyAsync(a=> (request.StartTime < a.EndTime && request.EndTime > a.StartTime), cancellationToken);
            if (hasConflict)
                throw new Exception("Time Slot Not Available.");

            var appointment = new Domain.Entities.Appointment
            {
                Id = Guid.NewGuid(),
                DoctorId = request.DoctorId,
                PatientId = request.PatientId,
                AppointmentDate = request.AppointmentDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Notes = request.Notes,
                Status = "Scheduled"
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(cancellationToken);
            return appointment.Id;
        }
    }

}
