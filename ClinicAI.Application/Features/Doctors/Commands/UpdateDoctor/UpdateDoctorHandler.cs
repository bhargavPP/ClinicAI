using ClinicAI.Application.common.Models;
using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Doctors.Commands.UpdateDoctor
{
    public class UpdateDoctorHandler
        : BaseHandler, IRequestHandler<UpdateDoctorCommand, Result<bool>>
    {
        public UpdateDoctorHandler(
            IClinicDbContext context,
            ICurrentUserService currentUser,
            IDateTime dateTime)
            : base(context, currentUser, dateTime)
        {
        }

        public async Task<Result<bool>> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (doctor == null)
                return Result<bool>.Failure("Doctor not found");

            // ✅ Basic validation
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<bool>.Failure("Name is required");

            if (string.IsNullOrWhiteSpace(request.Email))
                return Result<bool>.Failure("Email is required");

            // ✅ Duplicate email check (exclude current doctor)
            var exists = await _context.Doctors
                .AnyAsync(d => d.Email == request.Email && d.Id != request.Id, cancellationToken);

            if (exists)
                return Result<bool>.Failure("Doctor with this email already exists");

            // ✅ Update
            doctor.Name = request.Name;
            doctor.Specialization = request.Specialization;
            doctor.Email = request.Email;
            doctor.Phone = request.Phone;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "Doctor updated successfully");
        }
    }
}