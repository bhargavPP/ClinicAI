using ClinicAI.Application.Features.Doctors.Commands.CreateDoctor;
using ClinicAI.Application.Features.Doctors.Commands.DeleteDoctor;
using ClinicAI.Application.Features.Doctors.Commands.UpdateDoctor;
using ClinicAI.Application.Features.Doctors.Queries.GetDoctors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAI.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DoctorsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetDoctorsQuery());
            return Ok(result);
        }
        [Authorize(Roles = "Doctor,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateDoctorCommand command)
        {
            
            var id = await _mediator.Send(command);
            return Ok(id);
        }
        [Authorize(Roles = "Doctor,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteDoctorCommand { Id = id });
            return Ok(result);
        }
        [Authorize(Roles = "Doctor,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateDoctorCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
