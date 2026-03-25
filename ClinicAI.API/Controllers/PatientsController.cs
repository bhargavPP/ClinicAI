using ClinicAI.Application.Features.Patients.Commands.CreatePatient;
using ClinicAI.Application.Features.Patients.Queries.GetPatients;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PatientsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePatientCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }
        [HttpGet]
        public async Task<IActionResult> GetPatients()
        {
            var result = await _mediator.Send(new GetPatientsQuery());
            return Ok(result);
        }
    }
}
