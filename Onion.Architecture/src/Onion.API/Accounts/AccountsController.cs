using Microsoft.AspNetCore.Mvc;
using Onion.API.Accounts.Extensions;
using Onion.API.Accounts.Models;
using Onion.API.Common;
using Onion.Application.Accounts.Interfaces;

namespace Onion.API.Accounts
{
    [Route("api/accounts")]
    public sealed class AccountsController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountViewModel viewModel, CancellationToken ct)
        {
            var request = viewModel.ToRequest();

            await _accountService.CreateAsync(request, ct);
            return Ok();
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(CancellationToken ct)
        {
            var result = await _accountService.GetSummaryAsync(ct);

            if (result is null) return NotFound();

            return Ok(result);
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetDetails(CancellationToken ct)
        {
            var result = await _accountService.GetDetailsAsync(ct);

            if (result is null) return NotFound();

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileViewModel viewModel, CancellationToken ct)
        {

            var request = viewModel.ToRequest();

            await _accountService.UpdateProfileAsync(request, ct);

            return NoContent();
        }
    }
}
