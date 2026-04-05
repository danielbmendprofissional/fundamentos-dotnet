using Microsoft.AspNetCore.Mvc;
using Onion.API.Common;
using Onion.API.Partners.Extensions;
using Onion.API.Partners.Models;
using Onion.Application.Partners.Interfaces;

namespace Onion.API.Partners
{
    [Route("api/[controller]")]
    public sealed class PartnersController : BaseController
    {
        private readonly IPartnerService _partnerService;

        public PartnersController(IPartnerService partnerService)
        {
            _partnerService = partnerService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePartnerViewModel viewModel, CancellationToken ct)
        {
            var request = viewModel.ToRequest();

            await _partnerService.CreateAsync(request, ct);

            return CreatedAtAction(nameof(Create), new { email = request.Email.Address }, null);
        }

        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] PartnerFilterViewModel viewModel, CancellationToken ct)
        {
            var filter = viewModel.ToRequest();
            var result = await _partnerService.GetFilteredListAsync(filter, ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetails(Guid id, CancellationToken ct)
        {
            var result = await _partnerService.GetDetailsAsync(id, ct);
            return result is not null ? Ok(result) : NotFound();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePartnerViewModel viewModel, CancellationToken ct)
        {
            var request = viewModel.ToRequest(id);
            await _partnerService.UpdateAsync(request, ct);
            return NoContent();
        }
    }
}
