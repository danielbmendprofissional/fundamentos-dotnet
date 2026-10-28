using Microsoft.AspNetCore.Mvc;
using Onion.API.Appointments.Extensions;
using Onion.API.Appointments.Models;
using Onion.API.Common;
using Onion.Application.Appointments.Interfaces;
using Onion.Application.Appointments.Services;

namespace Onion.API.Appointments
{
    [Route("api/[controller]")]
    public sealed class AppointmentsController : BaseController
    {
        private readonly IAppointmentService _service;

        public AppointmentsController(
            IAppointmentService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentViewModel viewModel, CancellationToken ct)
        {
            var request = viewModel.ToRequest();
            await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(Create), new { name = request.Name }, null);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAppointmentViewModel viewModel, CancellationToken ct)
        {
            var request = viewModel.ToRequest(id);

            await _service.UpdateAsync(request, ct);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] AppointmentFilterViewModel viewModel, CancellationToken ct)
        {
            var filter = viewModel.ToRequest();

            var result = await _service.GetFilteredListAsync(filter, ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetails(Guid id, CancellationToken ct)
        {
            var result = await _service.GetDetailsAsync(id, ct);
            return result is not null ? Ok(result) : NotFound();
        }

        [HttpGet("{id:guid}/address")]
        public async Task<IActionResult> GetAddress(Guid id, CancellationToken ct)
        {
            var result = await _service.GetAddressDetailsAsync(id, ct);
            return result is not null ? Ok(result) : NotFound();
        }

        [HttpGet("{id:guid}/dates/{dateId:guid}")]
        public async Task<IActionResult> GetDateDetails(Guid id, Guid dateId, CancellationToken ct)
        {
            var result = await _service.GetAppointmentDateDetailsAsync(id, dateId, ct);
            return result is not null ? Ok(result) : NotFound();
        }

        [HttpGet("{id:guid}/participants/{participantId:guid}")]
        public async Task<IActionResult> GetParticipantDetails(Guid id, Guid participantId, CancellationToken ct)
        {
            var result = await _service.GetParticipantDetailsAsync(id, participantId, ct);
            return result is not null ? Ok(result) : NotFound();
        }
    }
}