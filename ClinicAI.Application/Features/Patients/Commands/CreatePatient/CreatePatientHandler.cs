using ClinicAI.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Patients.Commands.CreatePatient
{
    public class CreatePatientHandler :IRequestHandler<CreatePatientCommand,Guid>
    {
        private readonly IClinicDbContext _context;
        public CreatePatientHandler(IClinicDbContext context)
        {
            _context = context??throw new ArgumentNullException(nameof(context));
        }
        public async Task<Guid> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = new Domain.Entities.Patient
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                DateOfBirth = request.DateOfBirth
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(cancellationToken);
            return patient.Id;
        }
    }
}
