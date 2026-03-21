using ClinicAI.Application.Features.Availability.Commands.CreateAvailability;
using ClinicAI.Application.Features.Availability.Commands.DeleteAvailability;
using ClinicAI.Application.Features.Availability.Commands.UpdateAvailability;
using ClinicAI.Application.Features.Availability.Queries.GetAvailability;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AvailabilityController(IMediator mediator)
        {
            _mediator = mediator??throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAvailability([FromBody] CreateAvailabilityCommand command)
        {
            try
            {
                var availabilityId = await _mediator.Send(command);
                return Ok(availabilityId);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // ✅ FIX: Add HttpGet + Filters
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? doctorId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = new GetAvailabilityQuery
            {
                DoctorId = doctorId,
                FromDate = fromDate,
                ToDate = toDate
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteAvailabilityCommand { Id = id });
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id,[FromBody] UpdateAvailabilityCommand command)
        {
            command.Id = id;
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }
           
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
