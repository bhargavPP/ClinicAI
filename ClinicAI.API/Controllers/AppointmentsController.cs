using ClinicAI.Application.Features.Appointments.Commands.CancelAppointment;
using ClinicAI.Application.Features.Appointments.Commands.CreateAppointment;
using ClinicAI.Application.Features.Appointments.Commands.UpdateAppointment;
using ClinicAI.Application.Features.Appointments.Queries.GetAppointments;
using ClinicAI.Application.Features.Appointments.Queries.GetDoctorSlots;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AppointmentsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(_mediator));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAppointmentCommand command)
        {
            var result = await _mediator.Send(command);
            //if (!result.IsSuccess)
            //    return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("slots")]
        public async Task<IActionResult> GetSlots(Guid doctorId, DateTime date)
        {
            var result = await _mediator.Send(new GetDoctorSlotsQuery
            {
                DoctorId = doctorId,
                Date = date
            });

            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAppointments([FromQuery] Guid? patientId)
        {
            var result = await _mediator.Send(new GetAppointmentsQuery
            {
                PatientId = patientId
            });
            return Ok(result);
        }
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var result = await _mediator.Send(new CancelAppointmentCommand { Id = id });
            //if (!result.IsSuccess)
            //    return BadRequest(result);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateAppointmentCommand command)
        {
            if (id != command.Id)
                return BadRequest("Invalid request");
            var result = await _mediator.Send(command);
            //if (!result.IsSuccess)
            //    return BadRequest(result);
            return Ok(result);
        }
    }
}
