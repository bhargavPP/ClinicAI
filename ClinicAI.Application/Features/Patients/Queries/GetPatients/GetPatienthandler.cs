using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Patients.Queries.GetPatients
{
    public class GetPatienthandler : IRequestHandler<GetPatientsQuery, List<PatientDto>>
    {
        private readonly IClinicDbContext _context;

        public GetPatienthandler(IClinicDbContext context)
        {
            _context = context;
        }

        public async Task<List<PatientDto>> Handle(
            GetPatientsQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Patients
                .Select(p => new PatientDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Email = p.Email,
                    Phone = p.Phone,
                    DateOfBirth = p.DateOfBirth
                })
                .ToListAsync(cancellationToken);
        }
    }
}
