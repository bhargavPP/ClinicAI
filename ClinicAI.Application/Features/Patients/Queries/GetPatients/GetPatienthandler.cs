using ClinicAI.Application.Interfaces;
using ClinicAI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Patients.Queries.GetPatients
{
    public class GetPatienthandler : IRequestHandler<GetPatientsQuery, List<PatientDto>>
    {
        private readonly IClinicDbContext _context;
        private readonly ICurrentUserService _currentUser;
        public GetPatienthandler(IClinicDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<PatientDto>> Handle(
            GetPatientsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;

            return await _context.Patients.Where(p => p.UserId == userId)
                .Select(p => new PatientDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Email = p.Email,
                    Phone = p.Phone,
                    DateOfBirth = p.DateOfBirth,
                    RelationshipToUser = p.RelationshipToUser
                })
                .ToListAsync(cancellationToken);
        }
    }
}
