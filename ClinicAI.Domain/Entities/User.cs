using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Domain.Entities
{
    public class User
    {
        public Guid id {  get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public List<Patient> Patients { get; set; }
    }
}
