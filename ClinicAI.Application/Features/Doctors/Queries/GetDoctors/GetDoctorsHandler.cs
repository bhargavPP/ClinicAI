using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Application.Features.Doctors.Queries.GetDoctors
{
    public class GetDoctorsHandler :IRequestHandler<GetDoctorsQuery, List<DoctorDto>>
    {
        private readonly IClinicDbContext _context;
        public GetDoctorsHandler(IClinicDbContext context)
        {
            _context = context??throw new ArgumentNullException(nameof(context));
        }
        public async Task<List<DoctorDto>> Handle(GetDoctorsQuery request, CancellationToken cancellationToken)
        {
            var doctors = await _context.Doctors.ToListAsync(cancellationToken);
            return doctors.Select(d => new DoctorDto
            {
                Id = d.Id,
                Name = d.Name,
                Specialization = d.Specialization,
                Email = d.Email,
                Phone = d.Phone
            }).ToList();
        }
    }
}
