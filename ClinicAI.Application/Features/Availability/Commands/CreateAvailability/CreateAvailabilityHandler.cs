using ClinicAI.Application.common.Models;
using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Availability.Commands.CreateAvailability
{
    public class CreateAvailabilityHandler :BaseHandler, IRequestHandler<CreateAvailabilityCommand, Result<Guid>>
    {
        public CreateAvailabilityHandler(IClinicDbContext context, ICurrentUserService currentUser, IDateTime dateTime)
            : base(context, currentUser, dateTime)
        {
        }
        public async Task<Result<Guid>> Handle(CreateAvailabilityCommand request, CancellationToken cancellationToken)
        {
            // ✅ Validate doctor
            var doctorExists = await _context.Doctors
                .AnyAsync(d => d.Id == request.DoctorId, cancellationToken);

            if (!doctorExists)
                return Result<Guid>.Failure("Doctor not found");

            // ✅ Validate time
            if (request.StartTime >= request.EndTime)
                return Result<Guid>.Failure("Start time must be before end time");

            // ❗ Prevent past date
            if (request.Date.Date < _dateTime.dateTimeUtcNow)
                return Result<Guid>.Failure("Cannot create availability in the past");

            // 🔥 Simplified overlap check
            var exists = await _context.DoctorsAvailabilities
                .Where(a => a.DoctorId == request.DoctorId &&
                            a.Date.Date == request.Date.Date)
                .AnyAsync(a => request.StartTime < a.EndTime &&
                               request.EndTime > a.StartTime,
                          cancellationToken);

            if (exists)
                return Result<Guid>.Failure("Overlapping availability exists");

            // ✅ Create
            var availability = new Domain.Entities.DoctorsAvailability
            {
                Id = Guid.NewGuid(),
                DoctorId = request.DoctorId,
                Date = request.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsAvailable = request.IsAvailable
            };

            _context.DoctorsAvailabilities.Add(availability);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(availability.Id, "Availability created successfully");
        }
    }
}
