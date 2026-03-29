using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Patients.Commands.DeletePatient
{
    public class DeletePatientHandler : BaseHandler, IRequestHandler<DeletePatientCommand, bool>
    {
        public DeletePatientHandler(
            IClinicDbContext context,
            ICurrentUserService currentUser,IDateTime dateTime)
            : base(context, currentUser,dateTime)
        {
        }
        public async Task<bool> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = await _context.Patients
           .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (patient == null)
            {
                throw new Exception("Doctor not found");
                //return false; // Doctor not found
            }
            patient.IsDeleted = true;
            patient.DeletedAt = _dateTime.dateTimeUtcNow;
            patient.DeletedBy = currentUserId;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
