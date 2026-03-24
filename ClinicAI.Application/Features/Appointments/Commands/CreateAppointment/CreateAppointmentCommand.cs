using ClinicAI.Application.common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Appointments.Commands.CreateAppointment
{
    public class CreateAppointmentCommand : IRequest<Result<Guid>>
    {
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        //public TimeSpan EndTime { get; set; }
        public String? Notes { get; set; }
    }
}
