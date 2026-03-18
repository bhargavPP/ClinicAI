using ClinicAI.Application.Features.Doctors.Commands.CreateDoctor;
using ClinicAI.Application.Features.Doctors.Queries.GetDoctors;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IMediator _mediator;
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =  await _mediator.Send(new GetDoctorsQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDoctorCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }
    }
}
