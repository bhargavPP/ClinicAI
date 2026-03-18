using MediatR;

namespace ClinicAI.Application.Features.Doctors.Queries.GetDoctors
{
    public class GetDoctorsQuery : IRequest<List<DoctorDto>>
    {
    }
}
