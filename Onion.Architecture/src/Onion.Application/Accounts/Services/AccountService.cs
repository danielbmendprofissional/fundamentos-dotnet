using FluentValidation;
using Onion.Application.Accounts.DTOs.Requests;
using Onion.Application.Accounts.DTOs.Responses;
using Onion.Application.Accounts.Extensions;
using Onion.Application.Accounts.Interfaces;
using Onion.Application.Common.Base;
using Onion.Application.Common.DTOs.Result;
using Onion.Application.Common.Exceptions;
using Onion.Application.Common.Extensions;
using Onion.Application.Common.Interfaces;
using Onion.Domain.Accounts.Interfaces;
using System.Security.Authentication;
using System.Security.Claims;

namespace Onion.Application.Accounts.Services
{
    public sealed class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        private readonly IAccountRepository _repository;
        private readonly IAccountQueryRepository _queryRepository;
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;
        private readonly IValidator<CreateAccountRequest> _createValidator;

        public AccountService(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
        
            IAccountRepository repository, 
            IAccountQueryRepository queryRepository,
            IIdentityService identityService,
            ITokenService tokenService,
            IValidator<CreateAccountRequest> createValidator)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;

            _repository = repository;
            _queryRepository = queryRepository;
            _identityService = identityService;
            _createValidator = createValidator;
            _tokenService = tokenService;
        }

        public async Task CreateAsync(CreateAccountRequest request, CancellationToken ct = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, ct);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage);

                throw new ApplicationValidationException(errors);
            }
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var (succeeded, userId, erros) = await _identityService.RegisterAsync(request.Email.Address, request.Password);

                if (!succeeded) throw new Exception("Falha ao criar identidade.");

                var account = request.ToEntity(userId);

                await _repository.AddAsync(account, ct);
                await _unitOfWork.CommitAsync(ct);
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

        }

        public async Task<AccountSummaryResponse?> GetSummaryAsync(CancellationToken ct = default)
        {
            var accountId = _userProvider.GetUserId();

            return await _queryRepository.GetSummary(accountId, ct);
        }

        public async Task<AccountDetailsResponse?> GetDetailsAsync(CancellationToken ct = default)
        {
            var accountId = _userProvider.GetUserId();

            return await _queryRepository.GetDetails(accountId, ct);
        }

        public async Task UpdateProfileAsync(UpdateProfileRequest request, CancellationToken ct = default)
        {
            var accountId = _userProvider.GetUserId();

            var account = await _repository.GetByIdAsync(accountId, ct);

            if (account is null)
                throw new Exception("Account not found");

            account.UpdateProfile(request.FullName.ToValueObject(), request.PhotoUrl);

            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var result = await _identityService.LoginAsync(request.Email, request.Password, request.RememberMe);

            if (result.RequiresTwoFactor) return new LoginResponse { RequiresTwoFactor = true };
            if (!result.Succeeded) throw new Exception(result.Errors?.FirstOrDefault());

            var claims = await GenerateClaimsAsync(result.User!);

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            return new LoginResponse(accessToken, refreshToken);
        }

        public async Task LogoutAsync()
        {
            var userId = _userProvider.GetUserId();
            if (userId != Guid.Empty)
            {
                await _identityService.LogoutAsync(userId);
            }
        }

        private async Task<IEnumerable<Claim>> GenerateClaimsAsync(UserResult user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("TokenVersion", user.TokenVersion.ToString())
            };

            var roles = await _identityService.GetUserRolesAsync(user.Id);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            return claims;
        }
    }
}
