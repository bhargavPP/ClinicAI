using ClinicAI.Application.common.Models;
using MediatR;

namespace ClinicAI.Application.Features.Appointments.Commands.UpdateAppointment
{
    public class UpdateAppointmentCommand:IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public Guid PatientId { get; set; }
        public TimeSpan StartTime { get; set; }
        public String? Notes { get; set; }
    }
}
