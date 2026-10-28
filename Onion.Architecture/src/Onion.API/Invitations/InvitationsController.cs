using Microsoft.AspNetCore.Mvc;
using Onion.API.Common;
using Onion.API.Invitations.Extensions;
using Onion.API.Invitations.Models;
using Onion.Application.Invitations.Interfaces;

namespace Onion.API.Invitations
{
    [Route("api/[controller]")]
    public sealed class InvitationsController : BaseController
    {
        private readonly IInvitationService _invitationService;

        public InvitationsController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInvitationViewModel viewModel, CancellationToken ct)
        {
            var request = viewModel.ToRequest();

            await _invitationService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(Create), new { email = request.RecipientEmail }, null);
        }

        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] InvitationFilterViewModel viewModel, CancellationToken ct)
        {
            var filter = viewModel.ToRequest();

            var result = await _invitationService.GetFilteredListAsync(filter, ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetails(Guid id, CancellationToken ct)
        {
            var result = await _invitationService.GetDetailsAsync(id, ct);
            return result is not null ? Ok(result) : NotFound();
        }

        [HttpPatch("{id:guid}/accept")]
        public async Task<IActionResult> Accept(Guid id, CancellationToken ct)
        {
            await _invitationService.AcceptAsync(id, ct);
            return NoContent();
        }

        [HttpPatch("{id:guid}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RespondInvitationViewModel viewModel, CancellationToken ct)
        {
            var request = viewModel.ToRequest(id);
            
            await _invitationService.RejectAsync(request, ct);
            return NoContent();
        }

        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] RespondInvitationViewModel viewModel, CancellationToken ct)
        {
            var request = viewModel.ToRequest(id);
            
            await _invitationService.CancelAsync(request, ct);
            return NoContent();
        }
    }
}
