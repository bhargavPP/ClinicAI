using MediatR;

namespace ClinicAI.Application.Features.Availability.Commands.DeleteAvailability
{ 
    public class DeleteAvailabilityCommand:IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
