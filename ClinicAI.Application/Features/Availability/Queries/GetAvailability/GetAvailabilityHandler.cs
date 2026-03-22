using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Availability.Queries.GetAvailability
{
    public class GetAvailabilityHandler :IRequestHandler<GetAvailabilityQuery, List<AvailabilityDto>>
    {
        private readonly IClinicDbContext _context;
        public GetAvailabilityHandler(IClinicDbContext context)
        {
            _context = context??throw new ArgumentNullException(nameof(context));
        }
        public async Task<List<AvailabilityDto>> Handle(GetAvailabilityQuery request, CancellationToken cancellationToken)
        {
            var query = _context.DoctorsAvailabilities.AsQueryable();

            // ✅ Filter by Doctor
            if (request.DoctorId!=null)
            {
                query = query.Where(x => x.DoctorId == request.DoctorId);
            }

            // ✅ Filter by From Date
            if (request.FromDate.HasValue)
            {
                query = query.Where(x => x.Date >= request.FromDate.Value);
            }

            // ✅ Filter by To Date
            if (request.ToDate.HasValue)
            {
                query = query.Where(x => x.Date <= request.ToDate.Value);
            }

            return await query
                .Select(x => new AvailabilityDto
                {
                    Id = x.Id,
                    DoctorId = x.DoctorId,
                    DoctorName = x.Doctor.Name,
                    Date = x.Date,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    IsAvailable = x.IsAvailable
                })
                .ToListAsync(cancellationToken);
        }
    }
}
