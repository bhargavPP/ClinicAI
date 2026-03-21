using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Availability.Commands.CreateAvailability
{
    public class CreateAvailabilityHandler : IRequestHandler<CreateAvailabilityCommand, Guid>
    {
        private readonly IClinicDbContext _context;
        public CreateAvailabilityHandler(IClinicDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Guid> Handle(CreateAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var exists = await _context.DoctorsAvailabilities.AnyAsync(a =>
                a.DoctorId == request.DoctorId &&
                a.Date == request.Date &&
                ((request.StartTime >= a.StartTime && request.StartTime < a.EndTime) ||
                 (request.EndTime > a.StartTime && request.EndTime <= a.EndTime) ||
                 (request.StartTime <= a.StartTime && request.EndTime >= a.EndTime)),
                cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException("The doctor already has an availability that overlaps with the specified time.");
            }

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
            return availability.Id;
        }
    }
}
