using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Patients.Commands.UpdatePatient
{
    public class UpdatePatientHandler :IRequestHandler<UpdatePatientCommand,bool>
    {
        private readonly IClinicDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public UpdatePatientHandler(
            IClinicDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == request.Id
                                      && p.UserId == _currentUser.UserId,
                                      cancellationToken);

            if (patient == null)
                throw new Exception("Patient not found or unauthorized");

            // 🔒 Optional rule: prevent changing Self relationship
            if (patient.RelationshipToUser == "Self" && request.RelationshipToUser != "Self")
                throw new Exception("Cannot change Self relationship");

            patient.Name = request.Name;
            patient.Email = request.Email;
            patient.Phone = request.Phone;
            patient.DateOfBirth = request.DateOfBirth;
            patient.RelationshipToUser = request.RelationshipToUser;
             patient.UpdatedBy = _currentUser.UserId;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
