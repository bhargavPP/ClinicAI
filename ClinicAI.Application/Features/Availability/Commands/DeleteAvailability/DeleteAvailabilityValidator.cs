using FluentValidation;

namespace ClinicAI.Application.Features.Availability.Commands.DeleteAvailability
{
    public class DeleteAvailabilityValidator:AbstractValidator<DeleteAvailabilityCommand>
    {
        public DeleteAvailabilityValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
