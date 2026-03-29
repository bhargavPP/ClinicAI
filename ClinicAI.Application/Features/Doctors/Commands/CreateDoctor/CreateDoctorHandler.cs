using ClinicAI.Application.common.Models;
using ClinicAI.Application.Interfaces;
using ClinicAI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Doctors.Commands.CreateDoctor
{
    public class CreateDoctorHandler :BaseHandler, IRequestHandler<CreateDoctorCommand, Result<Guid>>
    {
        public CreateDoctorHandler(IClinicDbContext context, ICurrentUserService currentUser, IDateTime dateTime)
            : base(context, currentUser, dateTime)
        {
        }

        public async Task<Result<Guid>> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
        {
            // ✅ Basic validation
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<Guid>.Failure("Name is required");

            if (string.IsNullOrWhiteSpace(request.Email))
                return Result<Guid>.Failure("Email is required");

            // ✅ Duplicate email check
            var exists = await _context.Doctors
                .AnyAsync(d => d.Email == request.Email, cancellationToken);

            if (exists)
                return Result<Guid>.Failure("Doctor with this email already exists");

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

            return Result<Guid>.Success(doctor.Id, "Doctor created successfully");
        }
    }
}
