using MediatR;

namespace ClinicAI.Application.Features.Appointments.Queries.GetDoctorSlots
{
    public class GetDoctorSlotsQuery : IRequest<List<TimeSpan>>
    {
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
    }
}
