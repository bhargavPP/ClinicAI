using MediatR;

namespace ClinicAI.Application.Features.Doctors.Commands.DeleteDoctor
{
    public class DeleteDoctorCommand:IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
