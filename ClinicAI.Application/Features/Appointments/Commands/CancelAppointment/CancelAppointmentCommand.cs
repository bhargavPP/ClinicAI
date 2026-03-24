using ClinicAI.Application.common.Models;
using MediatR;

namespace ClinicAI.Application.Features.Appointments.Commands.CancelAppointment
{
    public class CancelAppointmentCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
    }
}
