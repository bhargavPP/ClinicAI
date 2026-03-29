using ClinicAI.Application.Features.Patients.Commands.UpdatePatient;
using FluentValidation;

namespace ClinicAI.Application.Features.Doctors.Commands.UpdateDoctor
{
    public class UpdateDoctorValidator : AbstractValidator<UpdateDoctorCommand>
    {
        public UpdateDoctorValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Specialization).NotEmpty().MaximumLength(100);
            RuleFor(x=>x.Email).NotEmpty().EmailAddress().MaximumLength(100);
            RuleFor(x => x.Phone).NotEmpty().MaximumLength(100);
        }
    }
}
