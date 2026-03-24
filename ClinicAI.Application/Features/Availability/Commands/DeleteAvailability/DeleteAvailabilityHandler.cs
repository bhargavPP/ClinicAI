using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Availability.Commands.DeleteAvailability
{
    public class DeleteAvailabilityHandler:IRequestHandler<DeleteAvailabilityCommand,bool>
    {
        private readonly IClinicDbContext _context;
        public DeleteAvailabilityHandler(IClinicDbContext context)
        {
            _context = context??throw new Exception(nameof(context));
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
            _context.DoctorsAvailabilities.Remove(doctor);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
