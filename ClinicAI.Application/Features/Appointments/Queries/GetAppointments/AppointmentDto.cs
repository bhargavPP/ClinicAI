using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Appointments.Queries.GetAppointments
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = "";
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; } = "";

        public string PatientName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
