using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Appointments.Queries.GetAppointments
{
    public class GetAppointmentsHandler :IRequestHandler<GetAppointmentsQuery, List<AppointmentDto>>
    {
        private readonly IClinicDbContext _context;
        public GetAppointmentsHandler(IClinicDbContext context)
        {
            _context= context??throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<AppointmentDto>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Appointments.Include(a=>a.Doctor).Include(a=>a.Patient).AsQueryable();
            if (request.UserId.HasValue && !string.IsNullOrEmpty(request.Role))
            {
                if (request.Role == "Doctor")
                {
                    query = query.Where(a => a.DoctorId == request.UserId.Value);
                }
                else if (request.Role == "Patient")
                {
                    query = query.Where(a => a.Patient.UserId == request.UserId.Value);
                }
                // Admin → no filter
            }

            // ✅ CASE 2: Admin filtering by patientId
            else if (request.PatientId.HasValue)
            {
                query = query.Where(a => a.PatientId == request.PatientId.Value);
            }
            var appointments = await query.OrderByDescending(a=>a.AppointmentDate).Select(a => new AppointmentDto
            {
                Id = a.Id,
                DoctorName = a.Doctor.Name,
                DoctorId=a.DoctorId,
                Date = a.AppointmentDate,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Notes = a.Notes,
                Status = a.Status,
                PatientId= a.PatientId.ToString(),
                PatientName = a.Patient.Name,
                Email = a.Patient.Email,
                Phone = a.Patient.Phone,
                DateOfBirth = a.Patient.DateOfBirth

            }).ToListAsync(cancellationToken);
            return appointments;
        }
    }
}
