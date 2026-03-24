using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClinicAI.Application.Constants;

namespace ClinicAI.Application.Features.Appointments.Queries.GetDoctorSlots
{
    public class GetDoctorSlotsHandler : IRequestHandler<GetDoctorSlotsQuery, List<TimeSpan>>
    {
        private readonly IClinicDbContext _context;
        public GetDoctorSlotsHandler(IClinicDbContext context)
        {
            _context = context;
        }

        public async Task<List<TimeSpan>> Handle(GetDoctorSlotsQuery request, CancellationToken cancellationToken)
        {
            var availability = await _context.DoctorsAvailabilities
                                     .FirstOrDefaultAsync(a =>
                                     a.DoctorId == request.DoctorId &&
                                     a.Date.Date == request.Date.Date &&
                                     a.IsAvailable, cancellationToken);
            if(availability==null)
            {
                return new List<TimeSpan>();
            }

            var appointments = await _context.Appointments.Where(a =>
                                    a.DoctorId == request.DoctorId &&
                                    a.AppointmentDate.Date == request.Date.Date)
                                    .ToListAsync(cancellationToken);

            var slots = new List<TimeSpan>();
            var current = availability.StartTime;

            while (current.Add(TimeSpan.FromMinutes(Constant.SlotDurationMinutes)) <= availability.EndTime)
            {
                var end = current.Add(TimeSpan.FromMinutes(Constant.SlotDurationMinutes));

                var isBooked = appointments.Any(a => current < a.EndTime && end > a.StartTime);

                if (!isBooked)
                    slots.Add(current);

                current = current.Add(TimeSpan.FromMinutes(Constant.SlotDurationMinutes));
            }

            return slots;
        }
    }
}
