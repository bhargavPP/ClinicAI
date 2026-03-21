using MediatR;

namespace ClinicAI.Application.Features.Availability.Queries.GetAvailability
{
    public class GetAvailabilityQuery : IRequest<List<AvailabilityDto>>
    {
        public Guid? DoctorId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
