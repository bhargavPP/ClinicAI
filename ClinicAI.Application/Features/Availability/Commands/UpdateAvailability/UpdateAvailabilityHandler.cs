using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Availability.Commands.UpdateAvailability
{
    public class UpdateAvailabilityHandler :IRequestHandler<UpdateAvailabilityCommand,bool>
    {
        private readonly IClinicDbContext _context;
        public UpdateAvailabilityHandler(IClinicDbContext context)
        {
            _context = context??throw new ArgumentException(nameof(context));
        }

        public async Task<bool> Handle(UpdateAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var doctor = await _context.DoctorsAvailabilities
                .FirstOrDefaultAsync(d=>d.Id==request.Id, cancellationToken);

            if(doctor == null)
            {
            throw new KeyNotFoundException($"Doctor not found.");
            }

            doctor.StartTime = request.StartTime;
            doctor.EndTime = request.EndTime;
            doctor.Date = request.Date;
            doctor.IsAvailable = request.IsAvailable;

            await _context.SaveChangesAsync(cancellationToken);

            return true;

        }
    }
}
