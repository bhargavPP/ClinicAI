using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Availability.Commands.CreateAvailability
{
    public class CreateAvailabilityCommand:IRequest<Guid>
    {
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }

    }
}
