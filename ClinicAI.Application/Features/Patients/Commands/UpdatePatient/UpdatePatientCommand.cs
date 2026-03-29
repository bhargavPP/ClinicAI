using MediatR;

namespace ClinicAI.Application.Features.Patients.Commands.UpdatePatient
{
    public class UpdatePatientCommand:IRequest<bool>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }

        public string RelationshipToUser { get; set; } = null!;
    }
}
