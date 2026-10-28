using Galo.Application.Common.Exceptions;
using Galo.Application.Common.Interfaces.Persistence;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Competicoes.Enums;
using Galo.Domain.Competicoes.Interfaces;
using Galo.Domain.Ingressos.Entities;
using Galo.Domain.Ingressos.Enums;
using Galo.Domain.Ingressos.Interfaces;
using Galo.Domain.Ingressos.Services;
using Galo.Domain.Ingressos.ValueObjects;
using Galo.Domain.Torcida.Enums;
using Galo.Domain.Torcida.Interfaces;
using MediatR;

namespace Galo.Application.Ingressos.Commands
{
    public sealed record ItemCompraRequest(string Setor, int Quantidade);

    // ---------------- Criar compra ----------------

    public sealed record CriarCompraCommand(Guid TorcedorId, Guid PartidaId, IReadOnlyList<ItemCompraRequest> Itens)
        : IRequest<Guid>;

    public sealed class CriarCompraCommandHandler : IRequestHandler<CriarCompraCommand, Guid>
    {
        private readonly ICompraIngressoRepository _compras;
        private readonly ITorcedorRepository _torcedores;
        private readonly IPartidaRepository _partidas;
        private readonly ICalculadoraDeDescontoService _desconto;
        private readonly IUnitOfWork _unitOfWork;

        public CriarCompraCommandHandler(ICompraIngressoRepository compras, ITorcedorRepository torcedores,
            IPartidaRepository partidas, ICalculadoraDeDescontoService desconto, IUnitOfWork unitOfWork)
        {
            _compras = compras;
            _torcedores = torcedores;
            _partidas = partidas;
            _desconto = desconto;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CriarCompraCommand request, CancellationToken cancellationToken)
        {
            if (request.Itens is null || request.Itens.Count == 0)
                throw new DomainException("Informe ao menos um item.");

            var torcedor = await _torcedores.ObterPorIdAsync(request.TorcedorId, cancellationToken)
                           ?? throw new NotFoundException("Torcedor", request.TorcedorId);
            if (torcedor.Status == StatusTorcedor.Bloqueado)
                throw new DomainException("Torcedor bloqueado não pode comprar ingressos.");

            var partida = await _partidas.ObterPorIdAsync(request.PartidaId, cancellationToken)
                          ?? throw new NotFoundException("Partida", request.PartidaId);
            if (partida.Status != StatusPartida.Agendada)
                throw new DomainException("A venda de ingressos só está disponível para partidas agendadas.");

            var compra = CompraIngresso.Criar(torcedor.Id, partida.Id);

            foreach (var item in request.Itens)
            {
                // O PREÇO vem da partida (servidor), nunca do cliente. Vira SNAPSHOT dentro do item.
                var setor = partida.ObterSetor(item.Setor);

                // Checagem "otimista" só para dar erro rápido. Quem garante de verdade o estoque é a RESERVA
                // (concorrência otimista no setor) - entre esta checagem e a reserva outro torcedor pode comprar.
                if (item.Quantidade > setor.Disponiveis)
                    throw new DomainException($"Ingressos insuficientes no setor {setor.Nome}: restam {setor.Disponiveis}.");

                compra.AdicionarItem(setor.Nome, setor.Preco, item.Quantidade);
            }

            // DOMAIN SERVICE decide o percentual; a ENTIDADE aplica.
            var percentual = _desconto.PercentualParaPlano(torcedor.Plano);
            if (percentual > 0) compra.AplicarDesconto(percentual);

            await _compras.AdicionarAsync(compra, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return compra.Id;
        }
    }

    // ---------------- Pagamento ----------------

    public sealed record IniciarPagamentoCommand(Guid CompraId, MetodoPagamento Metodo) : IRequest<Guid>;

    public sealed class IniciarPagamentoCommandHandler : IRequestHandler<IniciarPagamentoCommand, Guid>
    {
        private readonly ICompraIngressoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public IniciarPagamentoCommandHandler(ICompraIngressoRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(IniciarPagamentoCommand request, CancellationToken cancellationToken)
        {
            var compra = await _repository.ObterPorIdAsync(request.CompraId, cancellationToken)
                         ?? throw new NotFoundException("Compra", request.CompraId);

            var pagamentoId = compra.IniciarPagamento(request.Metodo);

            _repository.Atualizar(compra);
            // Ao gravar, o PagamentoIniciadoEvent dispara o handler que publica o pedido de reserva no RabbitMQ.
            await _unitOfWork.CommitAsync(cancellationToken);
            return pagamentoId;
        }
    }

    public sealed record ConfirmarPagamentoCommand(Guid CompraId, Guid PagamentoId) : IRequest;

    public sealed class ConfirmarPagamentoCommandHandler : IRequestHandler<ConfirmarPagamentoCommand>
    {
        private readonly ICompraIngressoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmarPagamentoCommandHandler(ICompraIngressoRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ConfirmarPagamentoCommand request, CancellationToken cancellationToken)
        {
            var compra = await _repository.ObterPorIdAsync(request.CompraId, cancellationToken)
                         ?? throw new NotFoundException("Compra", request.CompraId);
            compra.ConfirmarPagamento(request.PagamentoId);
            _repository.Atualizar(compra);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }

    public sealed record RecusarPagamentoCommand(Guid CompraId, Guid PagamentoId) : IRequest;

    public sealed class RecusarPagamentoCommandHandler : IRequestHandler<RecusarPagamentoCommand>
    {
        private readonly ICompraIngressoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RecusarPagamentoCommandHandler(ICompraIngressoRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RecusarPagamentoCommand request, CancellationToken cancellationToken)
        {
            var compra = await _repository.ObterPorIdAsync(request.CompraId, cancellationToken)
                         ?? throw new NotFoundException("Compra", request.CompraId);
            compra.RecusarPagamento(request.PagamentoId);
            _repository.Atualizar(compra);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }

    // ---------------- Emissão / cancelamento ----------------

    public sealed record EmitirIngressosCommand(Guid CompraId) : IRequest<IReadOnlyCollection<string>>;

    public sealed class EmitirIngressosCommandHandler : IRequestHandler<EmitirIngressosCommand, IReadOnlyCollection<string>>
    {
        private readonly ICompraIngressoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public EmitirIngressosCommandHandler(ICompraIngressoRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyCollection<string>> Handle(EmitirIngressosCommand request, CancellationToken cancellationToken)
        {
            var compra = await _repository.ObterPorIdAsync(request.CompraId, cancellationToken)
                         ?? throw new NotFoundException("Compra", request.CompraId);
            var codigos = compra.EmitirIngressos();
            _repository.Atualizar(compra);
            await _unitOfWork.CommitAsync(cancellationToken);
            return codigos;
        }
    }

    public sealed record CancelarCompraCommand(Guid CompraId, string MotivoCodigo = "TorcedorDesistiu") : IRequest;

    public sealed class CancelarCompraCommandHandler : IRequestHandler<CancelarCompraCommand>
    {
        private readonly ICompraIngressoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelarCompraCommandHandler(ICompraIngressoRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CancelarCompraCommand request, CancellationToken cancellationToken)
        {
            var compra = await _repository.ObterPorIdAsync(request.CompraId, cancellationToken)
                         ?? throw new NotFoundException("Compra", request.CompraId);
            compra.Cancelar(MotivoCancelamento.De(request.MotivoCodigo));
            _repository.Atualizar(compra);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
