using ClinicAI.Application.Interfaces;
using MediatR;

namespace ClinicAI.Application.Features.Patients.Commands.CreatePatient
{
    public class CreatePatientHandler : BaseHandler, IRequestHandler<CreatePatientCommand, Guid>
    {
        public CreatePatientHandler(IClinicDbContext context, ICurrentUserService currentUser,IDateTime dateTime) 
            : base(context, currentUser,dateTime)
        {
        }
        public async Task<Guid> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = new Domain.Entities.Patient
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                DateOfBirth = request.DateOfBirth,
                UserId = currentUserId,
                RelationshipToUser = request.RelationshipToUser,
                CreatedBy = currentUserId,
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(cancellationToken);
            return patient.Id;
        }
    }
}
