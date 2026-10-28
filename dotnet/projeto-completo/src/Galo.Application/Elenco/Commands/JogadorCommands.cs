using Galo.Application.Common.Exceptions;
using Galo.Application.Common.Interfaces.Persistence;
using Galo.Domain.Elenco.Entities;
using Galo.Domain.Elenco.Enums;
using Galo.Domain.Elenco.Interfaces;
using Galo.Domain.Elenco.ValueObjects;
using MediatR;

namespace Galo.Application.Elenco.Commands
{
    // COMMAND = intenção de mudar estado (verbo no imperativo). Um handler por command.
    // Padrão de TODO handler de escrita: carregar agregado -> chamar método do domínio -> Commit.
    // A regra de negócio fica no DOMÍNIO; o handler só orquestra.

    public sealed record CadastrarJogadorCommand(
        string Nome, string? Apelido, int Camisa, Posicao Posicao,
        DateOnly DataNascimento, string Nacionalidade, decimal ValorMercado) : IRequest<Guid>;

    public sealed class CadastrarJogadorCommandHandler : IRequestHandler<CadastrarJogadorCommand, Guid>
    {
        private readonly IJogadorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CadastrarJogadorCommandHandler(IJogadorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CadastrarJogadorCommand request, CancellationToken cancellationToken)
        {
            // Value Objects validam na criação: dado inválido nem chega a existir.
            var jogador = Jogador.Criar(
                NomeJogador.Create(request.Nome),
                request.Apelido,
                NumeroCamisa.Create(request.Camisa),
                request.Posicao,
                request.DataNascimento,
                request.Nacionalidade,
                ValorMercado.Create(request.ValorMercado));

            await _repository.AdicionarAsync(jogador, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return jogador.Id;
        }
    }

    public sealed record AtualizarValorMercadoCommand(Guid JogadorId, decimal NovoValor) : IRequest;

    public sealed class AtualizarValorMercadoCommandHandler : IRequestHandler<AtualizarValorMercadoCommand>
    {
        private readonly IJogadorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AtualizarValorMercadoCommandHandler(IJogadorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AtualizarValorMercadoCommand request, CancellationToken cancellationToken)
        {
            var jogador = await _repository.ObterPorIdAsync(request.JogadorId, cancellationToken)
                          ?? throw new NotFoundException("Jogador", request.JogadorId);

            jogador.AtualizarValorMercado(ValorMercado.Create(request.NovoValor));

            _repository.Atualizar(jogador);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }

    public sealed record RegistrarContratoCommand(
        Guid JogadorId, DateOnly Inicio, DateOnly Fim, decimal SalarioMensal, decimal? ClausulaRescisoria) : IRequest<Guid>;

    public sealed class RegistrarContratoCommandHandler : IRequestHandler<RegistrarContratoCommand, Guid>
    {
        private readonly IJogadorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RegistrarContratoCommandHandler(IJogadorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(RegistrarContratoCommand request, CancellationToken cancellationToken)
        {
            // O repositório carrega o agregado COMPLETO (jogador + contratos): a regra de sobreposição
            // de contratos precisa enxergar os contratos existentes.
            var jogador = await _repository.ObterPorIdAsync(request.JogadorId, cancellationToken)
                          ?? throw new NotFoundException("Jogador", request.JogadorId);

            var contratoId = jogador.RegistrarContrato(request.Inicio, request.Fim, request.SalarioMensal, request.ClausulaRescisoria);

            _repository.Atualizar(jogador);
            await _unitOfWork.CommitAsync(cancellationToken);
            return contratoId;
        }
    }

    public sealed record LesionarJogadorCommand(Guid JogadorId, string Descricao) : IRequest;

    public sealed class LesionarJogadorCommandHandler : IRequestHandler<LesionarJogadorCommand>
    {
        private readonly IJogadorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public LesionarJogadorCommandHandler(IJogadorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(LesionarJogadorCommand request, CancellationToken cancellationToken)
        {
            var jogador = await _repository.ObterPorIdAsync(request.JogadorId, cancellationToken)
                          ?? throw new NotFoundException("Jogador", request.JogadorId);

            jogador.Lesionar(request.Descricao);

            _repository.Atualizar(jogador);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }

    public sealed record RecuperarJogadorCommand(Guid JogadorId) : IRequest;

    public sealed class RecuperarJogadorCommandHandler : IRequestHandler<RecuperarJogadorCommand>
    {
        private readonly IJogadorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RecuperarJogadorCommandHandler(IJogadorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RecuperarJogadorCommand request, CancellationToken cancellationToken)
        {
            var jogador = await _repository.ObterPorIdAsync(request.JogadorId, cancellationToken)
                          ?? throw new NotFoundException("Jogador", request.JogadorId);

            jogador.Recuperar();

            _repository.Atualizar(jogador);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
