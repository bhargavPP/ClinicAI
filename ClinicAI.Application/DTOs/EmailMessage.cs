using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.DTOs
{
    public class EmailMessage
    {
        public string ToEmail { get; set; }=default!;
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
    }
}
