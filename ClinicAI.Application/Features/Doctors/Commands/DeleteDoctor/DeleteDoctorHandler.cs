using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Doctors.Commands.DeleteDoctor
{
    public class DeleteDoctorHandler:IRequestHandler<DeleteDoctorCommand,bool>
    {
        private readonly IClinicDbContext _context;
        public DeleteDoctorHandler(IClinicDbContext context)
        {
            _context = context??throw new Exception(nameof(context));
        }

        public async Task<bool> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctor = await _context.Doctors
           .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (doctor == null)
            {
                throw new Exception("Doctor not found");
                //return false; // Doctor not found
            }
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
