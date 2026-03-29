using FluentValidation;

namespace ClinicAI.Application.Features.Patients.Commands.DeletePatient
{
    public class DeletePatientValidator:AbstractValidator<DeletePatientCommand>
    {
        public DeletePatientValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
