using FluentValidation;

namespace ClinicAI.Application.Features.Availability.Commands.UpdateAvailability
{
    public class UpdateAvailabilityValidator:AbstractValidator<UpdateAvailabilityCommand>
    {
        public UpdateAvailabilityValidator()
        {
            RuleFor(x => x.DoctorId).NotEmpty().WithMessage("DoctorId is required.");
            RuleFor(x => x.Date).NotEmpty().WithMessage("Date is required.");
            RuleFor(x => x.StartTime).NotEmpty().WithMessage("StartTime is required.");
            RuleFor(x => x.EndTime).NotEmpty().WithMessage("EndTime is required.");
            RuleFor(x => x.EndTime)
               .GreaterThan(x => x.StartTime)
               .WithMessage("EndTime must be greater than StartTime.");
            RuleFor(x => x.Date)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("Date cannot be in the past.");
            RuleFor(x => x.IsAvailable).NotNull().WithMessage("IsAvailable is required.");
        }
    }
}
