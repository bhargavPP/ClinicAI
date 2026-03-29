using MediatR;

namespace ClinicAI.Application.Features.Patients.Commands.DeletePatient
{
    public class DeletePatientCommand:IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
