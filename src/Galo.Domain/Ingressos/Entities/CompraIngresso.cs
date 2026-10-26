using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;
using Galo.Domain.Ingressos.Enums;
using Galo.Domain.Ingressos.Events;
using Galo.Domain.Ingressos.ValueObjects;

namespace Galo.Domain.Ingressos.Entities
{
    /// <summary>
    /// Aggregate Root da compra de ingressos de UMA partida.
    ///
    /// Fluxo feliz:
    ///   Criar -> AdicionarItem -> [AplicarDesconto] -> IniciarPagamento
    ///   -> (assíncrono, via RabbitMQ) ConfirmarReserva -> ConfirmarPagamento -> EmitirIngressos
    ///
    /// Referencia Torcedor e Partida só por Id (agregados diferentes = transações diferentes).
    /// A consistência entre Compra e estoque da Partida é EVENTUAL (integration events).
    /// </summary>
    public sealed class CompraIngresso : AggregateRoot
    {
        public Guid TorcedorId { get; private set; }
        public Guid PartidaId { get; private set; }
        public string Numero { get; private set; } = string.Empty;
        public decimal ValorTotal { get; private set; }
        public StatusCompra Status { get; private set; }

        /// <summary>true quando a Partida confirmou a reserva dos ingressos (estoque já foi baixado).</summary>
        public bool ReservaConfirmada { get; private set; }
        public MotivoCancelamento? MotivoCancelamento { get; private set; }

        private readonly List<ItemIngresso> _itens = new();
        public IReadOnlyCollection<ItemIngresso> Itens => _itens.AsReadOnly();

        private readonly List<PagamentoIngresso> _pagamentos = new();
        public IReadOnlyCollection<PagamentoIngresso> Pagamentos => _pagamentos.AsReadOnly();

        private CompraIngresso() { } // EF Core

        private CompraIngresso(Guid torcedorId, Guid partidaId)
        {
            Guard.AgainstEmptyGuid(torcedorId, nameof(torcedorId));
            Guard.AgainstEmptyGuid(partidaId, nameof(partidaId));

            TorcedorId = torcedorId;
            PartidaId = partidaId;
            Status = StatusCompra.Pendente;
            Numero = $"ING-{Id.ToString()[..8].ToUpperInvariant()}";
        }

        public static CompraIngresso Criar(Guid torcedorId, Guid partidaId) => new(torcedorId, partidaId);

        public int TotalDeIngressos => _itens.Sum(i => i.Quantidade);

        // ---------- Carrinho ----------

        public void AdicionarItem(string setor, decimal precoUnitario, int quantidade)
        {
            Guard.Against<DomainException>(Status != StatusCompra.Pendente,
                "Itens só podem ser adicionados enquanto a compra está pendente.");

            var existente = _itens.FirstOrDefault(i => i.Setor.Equals(setor?.Trim(), StringComparison.OrdinalIgnoreCase));
            if (existente is not null)
                existente.AdicionarUnidades(quantidade);
            else
                _itens.Add(new ItemIngresso(Id, setor!, precoUnitario, quantidade));

            RecalcularValorTotal();
            SetDataAtualizacao();
        }

        public void RemoverItem(Guid itemId)
        {
            Guard.Against<DomainException>(Status != StatusCompra.Pendente,
                "Itens só podem ser removidos enquanto a compra está pendente.");

            var item = _itens.FirstOrDefault(i => i.Id == itemId);
            Guard.Against<DomainException>(item is null, "Item não encontrado na compra.");

            _itens.Remove(item!);
            RecalcularValorTotal();
            SetDataAtualizacao();
        }

        /// <summary>Desconto de sócio-torcedor (percentual calculado pelo domain service) sobre todos os itens.</summary>
        public void AplicarDesconto(decimal porcentagem)
        {
            Guard.Against<DomainException>(Status != StatusCompra.Pendente,
                "O desconto só pode ser aplicado enquanto a compra está pendente.");

            foreach (var item in _itens) item.AplicarDesconto(porcentagem);

            RecalcularValorTotal();
            SetDataAtualizacao();
        }

        // ---------- Pagamento ----------

        public Guid IniciarPagamento(MetodoPagamento metodo)
        {
            Guard.Against<DomainException>(!_itens.Any(), "Não é possível iniciar o pagamento de uma compra sem itens.");
            Guard.Against<DomainException>(Status != StatusCompra.Pendente,
                "O pagamento só pode ser iniciado para compras pendentes.");

            var pagamento = new PagamentoIngresso(Id, metodo, ValorTotal);
            pagamento.DefinirCodigoTransacao(null);
            _pagamentos.Add(pagamento);

            Status = StatusCompra.AguardandoPagamento;
            SetDataAtualizacao();

            AddDomainEvent(new PagamentoIniciadoEvent(Id, pagamento.Id, PartidaId, ValorTotal, SnapshotItens()));
            return pagamento.Id;
        }

        /// <summary>Chamado quando a Partida confirma (via mensageria) que baixou o estoque dos setores.</summary>
        public void ConfirmarReserva()
        {
            Guard.Against<DomainException>(Status != StatusCompra.AguardandoPagamento,
                "A reserva só pode ser confirmada para compras aguardando pagamento.");

            ReservaConfirmada = true;
            SetDataAtualizacao();
        }

        public void ConfirmarPagamento(Guid pagamentoId)
        {
            Guard.Against<DomainException>(Status != StatusCompra.AguardandoPagamento,
                "A compra não está aguardando pagamento.");
            Guard.Against<DomainException>(!ReservaConfirmada,
                "Os ingressos ainda não foram reservados; aguarde a confirmação da reserva.");

            var pagamento = ObterPagamento(pagamentoId);
            pagamento.Confirmar();

            Status = StatusCompra.PagamentoConfirmado;
            SetDataAtualizacao();

            AddDomainEvent(new PagamentoConfirmadoEvent(Id, pagamento.Id, TorcedorId, pagamento.CodigoTransacao!, pagamento.Valor));
        }

        /// <summary>Pagamento recusado: a compra volta para Pendente (a reserva é mantida) e pode tentar de novo.</summary>
        public void RecusarPagamento(Guid pagamentoId)
        {
            Guard.Against<DomainException>(Status != StatusCompra.AguardandoPagamento,
                "A compra não está aguardando pagamento.");

            var pagamento = ObterPagamento(pagamentoId);
            pagamento.Recusar();

            Status = StatusCompra.Pendente;
            SetDataAtualizacao();

            AddDomainEvent(new PagamentoRecusadoEvent(Id, pagamento.Id, pagamento.CodigoTransacao!));
        }

        // ---------- Emissão e cancelamento ----------

        public IReadOnlyCollection<string> EmitirIngressos()
        {
            Guard.Against<DomainException>(Status != StatusCompra.PagamentoConfirmado,
                "Os ingressos só podem ser emitidos após a confirmação do pagamento.");

            Status = StatusCompra.IngressosEmitidos;
            SetDataAtualizacao();

            // um código por ingresso (poderia virar QR Code)
            var codigos = _itens
                .SelectMany(i => Enumerable.Range(1, i.Quantidade)
                    .Select(n => $"{Numero}-{i.Setor.ToUpperInvariant().Replace(' ', '-')}-{n:000}"))
                .ToList();

            AddDomainEvent(new IngressosEmitidosEvent(Id, TorcedorId, PartidaId, codigos));
            return codigos;
        }

        public void Cancelar(MotivoCancelamento? motivo = null)
        {
            Guard.Against<DomainException>(Status == StatusCompra.Cancelada, "A compra já está cancelada.");
            Guard.Against<DomainException>(Status >= StatusCompra.IngressosEmitidos,
                "Não é possível cancelar uma compra com ingressos já emitidos.");

            var motivoFinal = motivo ?? MotivoCancelamento.Outro();

            Status = StatusCompra.Cancelada;
            MotivoCancelamento = motivoFinal;
            SetDataAtualizacao();

            // O handler do evento só devolve o estoque se a reserva chegou a ser confirmada.
            AddDomainEvent(new CompraCanceladaEvent(Id, TorcedorId, PartidaId, motivoFinal, ReservaConfirmada, SnapshotItens()));
        }

        // ---------- helpers ----------

        private void RecalcularValorTotal() => ValorTotal = _itens.Sum(i => i.ValorTotal);

        private IReadOnlyCollection<ItemCompraSnapshot> SnapshotItens()
            => _itens.Select(i => new ItemCompraSnapshot(i.Setor, i.Quantidade)).ToList();

        private PagamentoIngresso ObterPagamento(Guid pagamentoId)
        {
            var pagamento = _pagamentos.FirstOrDefault(p => p.Id == pagamentoId);
            Guard.Against<DomainException>(pagamento is null, "Pagamento não encontrado na compra.");
            return pagamento!;
        }
    }
}
