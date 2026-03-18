using MediatR;
using ClinicAI.Domain.Entities;
using ClinicAI.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Doctors.Commands.CreateDoctor
{
    public class CreateDoctorHandler :IRequestHandler<CreateDoctorCommand, Guid>
    {
        private readonly IClinicDbContext _context;
        public CreateDoctorHandler(IClinicDbContext context)
        {
             _context = context??throw new ArgumentNullException(nameof(context));
        }
        public async Task<Guid> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctor = new Doctor
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Specialization = request.Specialization,
                Email = request.Email,
                Phone = request.Phone
            };
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync(cancellationToken);
            return doctor.Id;
        }
    }
}
