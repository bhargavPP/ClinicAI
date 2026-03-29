using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Availability.Commands.DeleteAvailability
{
    public class DeleteAvailabilityHandler:BaseHandler, IRequestHandler<DeleteAvailabilityCommand,bool>
    {
   
        public DeleteAvailabilityHandler(IClinicDbContext context,ICurrentUserService currentUser, IDateTime dateTime)
            : base(context, currentUser, dateTime)
        {
        }

        public async Task<bool> Handle(DeleteAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var doctor = await _context.DoctorsAvailabilities
           .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (doctor == null)
            {
                throw new Exception("Doctor not found");
                //return false; // Doctor not found
            }
            doctor.IsDeleted = true;
            doctor.DeletedAt = _dateTime.dateTimeUtcNow;
            doctor.DeletedBy = currentUserId;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
