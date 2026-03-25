using MediatR;

namespace ClinicAI.Application.Features.Patients.Queries.GetPatients
{
    public class GetPatientsQuery:IRequest<List<PatientDto>>
    {
    }
}
