using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;
using Vendas.Domain.Pedidos.Enums;
using Vendas.Domain.Pedidos.Events.Pagamento;
using Vendas.Domain.Pedidos.Events.Pedido;
using Vendas.Domain.Pedidos.Interfaces;
using Vendas.Domain.Pedidos.Snapshot;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Domain.Pedidos.Entities
{
    //sealed: não ser alterada e nem herdada.
    //AggregateRoot: clarear o domínio e conseguir controlar melhor como por exemplo
    //IRepository only accepts AggregateRoot type
    public sealed class Pedido : AggregateRoot
    {
        public Guid ClienteId { get; private set; }
        public EnderecoEntrega EnderecoEntrega { get; private set; }
        public decimal ValorTotal { get; private set; }
        public StatusPedido StatusPedido { get; private set; }
        public string NumeroPedido { get; private set; } = string.Empty;

        private readonly List<ItemPedido> _itens = new();
        public IReadOnlyCollection<IItemPedido> Itens => _itens.Cast<IItemPedido>().ToList().AsReadOnly();

        private readonly List<Pagamento> _pagamentos = new();
        public IReadOnlyCollection<IPagamento> Pagamentos => _pagamentos.Cast<IPagamento>().ToList().AsReadOnly();

        private Pedido(Guid clienteId, EnderecoEntrega enderecoEntrega)
        {
            Guard.AgainstEmptyGuid(clienteId, nameof(clienteId));
            Guard.AgainstNull(enderecoEntrega, nameof(enderecoEntrega));

            ClienteId = clienteId;
            EnderecoEntrega = enderecoEntrega;
            StatusPedido = StatusPedido.Pendente;
            ValorTotal = 0m;

            GerarNumeroPedido();
        }

        public static Pedido Criar(Guid clienteId, EnderecoEntrega enderecoEntrega)
            => new(clienteId, enderecoEntrega);

        public void AdicionarItem(Guid produtoId, string nomeProduto, decimal precoUnitario, int quantidade)
        {
            Guard.Against<DomainException>(
                StatusPedido != StatusPedido.Pendente,
                "Itens só podem ser adicionados enquanto o pedido está pendente.");

            var existente = _itens.FirstOrDefault(i => i.ProdutoId == produtoId);

            if (existente is not null)
                existente.AdicionarUnidades(quantidade);
            else
                _itens.Add(new ItemPedido(produtoId, nomeProduto, precoUnitario, quantidade));

            RecalcularValorTotal();
            SetDataAtualizacao();
        }

        public void RemoverItem(Guid itemId)
        {
            Guard.AgainstEmptyGuid(itemId, nameof(itemId));
            Guard.Against<DomainException>(
                StatusPedido != StatusPedido.Pendente,
                "Itens só podem ser removidos em pedidos pendentes.");

            var item = _itens.FirstOrDefault(i => i.Id == itemId);
            Guard.AgainstNull(item, nameof(item));

            _itens.Remove(item!);

            Guard.Against<DomainException>(
                _itens.Count == 0,
                "O pedido deve conter pelo menos um item.");

            RecalcularValorTotal();
            SetDataAtualizacao();
        }

        public void AtualizarEnderecoEntrega(EnderecoEntrega novoEndereco)
        {
            Guard.AgainstNull(novoEndereco, nameof(novoEndereco));
            Guard.Against<DomainException>(
                StatusPedido != StatusPedido.Pendente,
                "O endereço só pode ser alterado enquanto o pedido está pendente.");

            EnderecoEntrega = novoEndereco;
            SetDataAtualizacao();
        }

        public Guid IniciarPagamento(MetodoPagamento metodoPagamento)
        {
            Guard.Against<DomainException>(
                !_itens.Any(),
                "Não é possível iniciar o pagamento de um pedido sem itens.");

            Guard.Against<DomainException>(
                StatusPedido != StatusPedido.Pendente,
                "O pagamento só pode ser iniciado a partir do status Pendente.");

            if (_pagamentos.Any(p => p.StatusPagamento == StatusPagamento.Pendente))
                throw new DomainException("Já existe um pagamento pendente para este pedido.");

            var novoPagamento = new Pagamento(Id, metodoPagamento, ValorTotal);
            _pagamentos.Add(novoPagamento);

            SetDataAtualizacao();
            return novoPagamento.Id;
        }

        public void AdicionarCustoFrete(decimal valor)
        {
            Guard.AgainstNull(valor, nameof(valor));
            Guard.Against<DomainException>(valor < 0, "O custo de frete não pode ser negativo.");

            ValorTotal += valor;
            SetDataAtualizacao();
        }

        public void DefinirCodigoTransacao(Guid pagamentoId, string? codigo = null)
        {
            var pagamento = ObterPagamento(pagamentoId);

            if (string.IsNullOrEmpty(codigo))
            {
                pagamento.GerarCodigoTransacaoLocal();
            }
            else
            {
                pagamento.DefinirCodigoTransacao(codigo);
            }

            SetDataAtualizacao();
        }

        public void ConfirmarPagamento(Guid pagamentoId)
        {
            var pagamento = ObterPagamento(pagamentoId);

            Guard.Against<DomainException>(
                StatusPedido != StatusPedido.Pendente,
                "O pedido não está no status esperado para confirmação de pagamento.");

            pagamento.ConfirmarPagamento();

            StatusPedido = StatusPedido.PagamentoConfirmado;
            SetDataAtualizacao();

            AddDomainEvent(new PagamentoConfirmadoEvent(
               PedidoId: Id,
               PagamentoId: pagamento.Id,
               CodigoTransacao: pagamento.CodigoTransacao!,
               Valor: pagamento.Valor,
               DataPagamento: pagamento.DataPagamento!.Value
           ));
        }

        public void RecusarPagamento(Guid pagamentoId)
        {
            var pagamento = ObterPagamento(pagamentoId);

            Guard.Against<DomainException>(
                StatusPedido != StatusPedido.Pendente,
                "O pedido não está no status esperado para confirmação de pagamento.");

            pagamento.RecusarPagamento();

            StatusPedido = StatusPedido.Cancelado;
            SetDataAtualizacao();

            AddDomainEvent(new PagamentoRejeitadoEvent(
               PedidoId: Id,
               PagamentoId: pagamento.Id,
               CodigoTransacao: pagamento.CodigoTransacao!,
               Valor: pagamento.Valor,
               DataPagamento: pagamento.DataPagamento!.Value
           ));
        }

        public void MarcarComoEmSeparacao()
        {
            Guard.Against<DomainException>(
                StatusPedido != StatusPedido.PagamentoConfirmado,
                "O pedido só pode ir para 'Em Separação' após o pagamento ser confirmado.");

            StatusPedido = StatusPedido.EmSeparacao;
            SetDataAtualizacao();

            AddDomainEvent(new PedidoEmSeparacaoEvent(
            Id,
                _itens.Select(i => new PedidoItemSnapshot(
                    i.ProdutoId,
                    i.Quantidade
                )).ToList()
            ));
        }

        public void MarcarComoEnviado()
        {
            Guard.Against<DomainException>(
                StatusPedido != StatusPedido.EmSeparacao,
                "O pedido só pode ser 'Enviado' após estar 'Em Separação'."
            );

            StatusPedido = StatusPedido.Enviado;
            SetDataAtualizacao();

            AddDomainEvent(
                new PedidoEnviadoEvent(
                    Id,
                    ClienteId,
                    EnderecoEntrega
                )
            );
        }

        public void MarcarComoEntregue()
        {
            Guard.Against<DomainException>(
                StatusPedido != StatusPedido.Enviado,
                "O pedido só pode ser marcado como 'Entregue' após ser 'Enviado'."
            );

            StatusPedido = StatusPedido.Entregue;
            SetDataAtualizacao();

            AddDomainEvent(
                new PedidoEntregueEvent(
                    Id,
                    ClienteId
                )
            );
        }

        public void CancelarPedido(MotivoCancelamento? motivo = null)
        {
            Guard.Against<DomainException>(
                StatusPedido >= StatusPedido.EmSeparacao,
                "Não é possível cancelar um pedido que já está em separação ou posterior."
            );

            StatusPedido = StatusPedido.Cancelado;
            SetDataAtualizacao();

            AddDomainEvent(
                new PedidoCanceladoEvent(
                    Id,
                    ClienteId,
                    StatusPedido,
                    motivo ?? MotivoCancelamento.Outro(),
                    _pagamentos.LastOrDefault()?.Id
                )
            );
        }


        private void RecalcularValorTotal()
            => ValorTotal = _itens.Sum(i => i.ValorTotal);

        private void GerarNumeroPedido()
            => NumeroPedido = $"PED-{Id.ToString()[..8].ToUpper()}";

        private Pagamento ObterPagamento(Guid pagamentoId)
        {
            var pagamento = _pagamentos.FirstOrDefault((i => i.Id == pagamentoId));
            Guard.AgainstNull(pagamento, nameof(pagamento));

            return pagamento!;
        }
    }

}
