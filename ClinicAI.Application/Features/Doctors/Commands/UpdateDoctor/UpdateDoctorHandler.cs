using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Doctors.Commands.UpdateDoctor
{
    public class UpdateDoctorHandler :IRequestHandler<UpdateDoctorCommand,bool>
    {
        private readonly IClinicDbContext _context;
        public UpdateDoctorHandler(IClinicDbContext context)
        {
            _context = context??throw new ArgumentException(nameof(context));
        }

        public async Task<bool> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d=>d.Id==request.Id, cancellationToken);

            if(doctor == null)
            {
            throw new KeyNotFoundException($"Doctor not found.");
            }

            doctor.Name = request.Name;
            doctor.Specialization = request.Specialization;
            doctor.Email = request.Email;
            doctor.Phone = request.Phone;

            await _context.SaveChangesAsync(cancellationToken);

            return true;

        }
    }
}
