using FluentValidation;

namespace ClinicAI.Application.Features.Patients.Commands.UpdatePatient
{
    public class UpdatePatientValidator:AbstractValidator<UpdatePatientCommand>
    {
        public UpdatePatientValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("Valid email is required");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Now)
                .WithMessage("Date of birth must be in the past");

            RuleFor(x => x.RelationshipToUser)
                .NotEmpty().WithMessage("Relationship is required");
        }
    }
}
