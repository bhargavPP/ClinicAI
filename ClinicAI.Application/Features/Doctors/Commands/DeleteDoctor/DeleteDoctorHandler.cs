using ClinicAI.Application.Features.Patients.Commands.DeletePatient;
using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Doctors.Commands.DeleteDoctor
{
    public class DeletePatientHandler:BaseHandler, IRequestHandler<DeletePatientCommand,bool>
    {
    
        public DeletePatientHandler(IClinicDbContext context,ICurrentUserService currentUser, IDateTime dateTime) : base(context, currentUser, dateTime)
        {
            
        }

        public async Task<bool> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            var doctor = await _context.Doctors
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
