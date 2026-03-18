using ClinicAI.Application.Features.Appointments.Commands.CreateAppointment;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AppointmentsController(IMediator mediator)
        {
            mediator = _mediator;
        }

        [HttpPost]
        public async  Task<IActionResult> Create(CreateAppointmentCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }
    }
}
