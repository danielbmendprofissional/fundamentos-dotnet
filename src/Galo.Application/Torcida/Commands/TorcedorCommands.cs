using Galo.Application.Common.Exceptions;
using Galo.Application.Common.Interfaces.Persistence;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Torcida.Entities;
using Galo.Domain.Torcida.Enums;
using Galo.Domain.Torcida.Interfaces;
using Galo.Domain.Torcida.ValueObjects;
using MediatR;

namespace Galo.Application.Torcida.Commands
{
    public sealed record CadastrarTorcedorCommand(
        string Nome, string Cpf, string Email, string? Telefone, DateOnly DataNascimento) : IRequest<Guid>;

    public sealed class CadastrarTorcedorCommandHandler : IRequestHandler<CadastrarTorcedorCommand, Guid>
    {
        private readonly ITorcedorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CadastrarTorcedorCommandHandler(ITorcedorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CadastrarTorcedorCommand request, CancellationToken cancellationToken)
        {
            var cpf = Cpf.Create(request.Cpf);
            var email = Email.Create(request.Email);

            // Unicidade é regra que precisa olhar o BANCO, então fica no handler (ou domain service) - não na entidade.
            // (O índice único no banco continua sendo a garantia final contra corrida.)
            if (await _repository.ExistePorCpfAsync(cpf, cancellationToken))
                throw new DomainException("Já existe um torcedor com este CPF.");
            if (await _repository.ExistePorEmailAsync(email, cancellationToken))
                throw new DomainException("Já existe um torcedor com este e-mail.");

            var torcedor = Torcedor.Cadastrar(
                NomeCompleto.Create(request.Nome), cpf, email,
                string.IsNullOrWhiteSpace(request.Telefone) ? null : Telefone.Create(request.Telefone),
                request.DataNascimento);

            await _repository.AdicionarAsync(torcedor, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return torcedor.Id;
        }
    }

    public sealed record AlterarPlanoSocioCommand(Guid TorcedorId, PlanoSocio Plano) : IRequest;

    public sealed class AlterarPlanoSocioCommandHandler : IRequestHandler<AlterarPlanoSocioCommand>
    {
        private readonly ITorcedorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AlterarPlanoSocioCommandHandler(ITorcedorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AlterarPlanoSocioCommand request, CancellationToken cancellationToken)
        {
            var torcedor = await _repository.ObterPorIdAsync(request.TorcedorId, cancellationToken)
                           ?? throw new NotFoundException("Torcedor", request.TorcedorId);
            torcedor.AlterarPlano(request.Plano);
            _repository.Atualizar(torcedor);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }

    public sealed record AdicionarEnderecoCommand(
        Guid TorcedorId, string Cep, string Logradouro, string Numero, string Bairro,
        string Cidade, string Estado, string Pais = "Brasil", string? Complemento = null, bool Principal = false) : IRequest<Guid>;

    public sealed class AdicionarEnderecoCommandHandler : IRequestHandler<AdicionarEnderecoCommand, Guid>
    {
        private readonly ITorcedorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AdicionarEnderecoCommandHandler(ITorcedorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(AdicionarEnderecoCommand request, CancellationToken cancellationToken)
        {
            var torcedor = await _repository.ObterPorIdAsync(request.TorcedorId, cancellationToken)
                           ?? throw new NotFoundException("Torcedor", request.TorcedorId);

            var enderecoId = torcedor.AdicionarEndereco(request.Cep, request.Logradouro, request.Numero, request.Bairro,
                request.Cidade, request.Estado, request.Pais, request.Complemento, request.Principal);

            _repository.Atualizar(torcedor);
            await _unitOfWork.CommitAsync(cancellationToken);
            return enderecoId;
        }
    }

    public sealed record DefinirEnderecoPrincipalCommand(Guid TorcedorId, Guid EnderecoId) : IRequest;

    public sealed class DefinirEnderecoPrincipalCommandHandler : IRequestHandler<DefinirEnderecoPrincipalCommand>
    {
        private readonly ITorcedorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DefinirEnderecoPrincipalCommandHandler(ITorcedorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DefinirEnderecoPrincipalCommand request, CancellationToken cancellationToken)
        {
            var torcedor = await _repository.ObterPorIdAsync(request.TorcedorId, cancellationToken)
                           ?? throw new NotFoundException("Torcedor", request.TorcedorId);
            torcedor.DefinirEnderecoPrincipal(request.EnderecoId);
            _repository.Atualizar(torcedor);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }

    public sealed record BloquearTorcedorCommand(Guid TorcedorId, string Motivo) : IRequest;

    public sealed class BloquearTorcedorCommandHandler : IRequestHandler<BloquearTorcedorCommand>
    {
        private readonly ITorcedorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public BloquearTorcedorCommandHandler(ITorcedorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(BloquearTorcedorCommand request, CancellationToken cancellationToken)
        {
            var torcedor = await _repository.ObterPorIdAsync(request.TorcedorId, cancellationToken)
                           ?? throw new NotFoundException("Torcedor", request.TorcedorId);
            torcedor.Bloquear(request.Motivo);
            _repository.Atualizar(torcedor);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
