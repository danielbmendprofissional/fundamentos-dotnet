using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onion.API.Accounts.Extensions;
using Onion.API.Accounts.Models;
using Onion.API.Common;
using Onion.Application.Accounts.Interfaces;
using Onion.Application.Accounts.Services;

namespace Onion.API.Accounts
{
    [Route("api/accounts")]
    public sealed class AccountsController : BaseController
    {
        private readonly IAccountService _service;

        public AccountsController(IAccountService accountService)
        {
            _service = accountService;
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAccountViewModel viewModel, CancellationToken ct)
        {
            var request = viewModel.ToRequest();

            await _service.CreateAsync(request, ct);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginViewModel viewModel, CancellationToken ct)
        {
            var request = viewModel.ToRequest();
            var response = await _service.LoginAsync(request, ct);

            if (response.RequiresTwoFactor)
                return Ok(new { RequiresTwoFactor = true, Message = "Código 2FA enviado." });

            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            await _service.LogoutAsync(ct);
            return NoContent();
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(CancellationToken ct)
        {
            var result = await _service.GetSummaryAsync(ct);

            if (result is null) return NotFound();

            return Ok(result);
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetDetails(CancellationToken ct)
        {
            var result = await _service.GetDetailsAsync(ct);

            if (result is null) return NotFound();

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileViewModel viewModel, CancellationToken ct)
        {

            var request = viewModel.ToRequest();

            await _service.UpdateProfileAsync(request, ct);

            return NoContent();
        }
    }
}
