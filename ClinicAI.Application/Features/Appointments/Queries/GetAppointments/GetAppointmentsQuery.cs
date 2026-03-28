using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Appointments.Queries.GetAppointments
{
    public class GetAppointmentsQuery : IRequest<List<AppointmentDto>>
    {
        public Guid? PatientId { get; set; }
        public string? Role { get; set; }
        public Guid? UserId { get; set; }
    }
}
