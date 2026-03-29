using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Availability.Commands.UpdateAvailability
{
    public class UpdateAvailabilityHandler : BaseHandler, IRequestHandler<UpdateAvailabilityCommand,bool>
    {
        public UpdateAvailabilityHandler(IClinicDbContext context, ICurrentUserService currentUser, IDateTime dateTime)
            : base(context, currentUser, dateTime)
        {
        }

        public async Task<bool> Handle(UpdateAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var availability = await _context.DoctorsAvailabilities
        .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (availability == null)
                throw new KeyNotFoundException("Availability not found");

            // ✅ Validate time
            if (request.StartTime >= request.EndTime)
                throw new Exception("Start time must be before end time");

            // ❗ Prevent past date
            if (request.Date.Date < _dateTime. dateTimeUtcNow)
                throw new Exception("Cannot update past availability");

            // 🔥 Overlap check (exclude current record)
            var overlap = await _context.DoctorsAvailabilities
                .Where(a => a.DoctorId == availability.DoctorId &&
                            a.Date.Date == request.Date.Date &&
                            a.Id != request.Id)
                .AnyAsync(a => request.StartTime < a.EndTime &&
                               request.EndTime > a.StartTime,
                          cancellationToken);

            if (overlap)
                throw new Exception("Overlapping availability exists");

            // ✅ Update
            availability.StartTime = request.StartTime;
            availability.EndTime = request.EndTime;
            availability.Date = request.Date;
            availability.IsAvailable = request.IsAvailable;

            await _context.SaveChangesAsync(cancellationToken);

            return true;

        }
    }
}
