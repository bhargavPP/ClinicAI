using ClinicAI.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Availability.Commands.UpdateAvailability
{ 
    public class UpdateAvailabilityCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
       
    }
}
