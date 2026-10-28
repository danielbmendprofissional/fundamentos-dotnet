using FluentValidation;
using Onion.Application.Accounts.DTOs.Requests;
using Onion.Application.Accounts.DTOs.Responses;
using Onion.Application.Accounts.Extensions;
using Onion.Application.Accounts.Interfaces;
using Onion.Application.Common.Base;
using Onion.Application.Common.Exceptions;
using Onion.Application.Common.Extensions;
using Onion.Application.Common.Interfaces;
using Onion.Domain.Accounts.Interfaces;

namespace Onion.Application.Accounts.Services
{
    public sealed class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        private readonly IAccountRepository _repository;
        private readonly IAccountQueryRepository _queryRepository;
        private readonly IValidator<CreateAccountRequest> _createValidator;

        public AccountService(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
        
            IAccountRepository repository, 
            IAccountQueryRepository queryRepository,
            IValidator<CreateAccountRequest> createValidator)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;

            _repository = repository;
            _queryRepository = queryRepository;
            _createValidator = createValidator;
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
                // TODO: Integrar com IIdentityClient (será implementado via padrão HttpClient)
                // 1. Chamar IIdentityClient.RegisterAsync(email, password)
                // 2. Obter userId da resposta
                // 3. Criar account local com os dados retornados

                var account = request.ToEntity(Guid.NewGuid());
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

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            // TODO: Integrar com IIdentityClient
            // 1. Chamar IIdentityClient.LoginAsync(email, password)
            // 2. Retornar token(s) diretamente da API de Identity

            throw new NotImplementedException("Aguardando implementação de IIdentityClient");
        }

        public async Task LogoutAsync(CancellationToken ct = default)
        {
            // TODO: Integrar com IIdentityClient
            // 1. Obter userId do IUserProvider
            // 2. Chamar IIdentityClient.LogoutAsync(userId)

              throw new NotImplementedException("Aguardando implementação de IIdentityClient");
        }
    }
}
