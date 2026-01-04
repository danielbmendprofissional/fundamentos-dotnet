using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Enum;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;
using Vendas.Domain.Events;
using Vendas.Domain.Events.Pagamento;

namespace Vendas.Domain.Entities
{
    public sealed class Pagamento : Entity
    {
        public Guid PedidoId { get; private set; }
        //Prefiro utilização de INDICADOR, mas é apenas um projeto DDD educacional
        public MetodoPagamento MetodoPagamento { get; private set; }
        public StatusPagamento StatusPagamento { get; private set; }
        public decimal Valor {  get; private set; } //Poderia criar um ValueObject
        public DateTime? DataPagamento { get; private set; }
        public string? CodigoTransacao { get; private set; }

        internal Pagamento(Guid pedidoId, MetodoPagamento metodoPagamento, decimal valor)
        {
            Guard.AgainstEmptyGuid(pedidoId, nameof(pedidoId));

            Guard.Against<DomainException>(
                valor <= 0,
                "O valor do pagamento deve ser maior que zero.");

            Guard.Against<DomainException>(
                !Enum.IsDefined(typeof(MetodoPagamento), metodoPagamento),
                "Método de pagamento inválido.");

            PedidoId = pedidoId;
            MetodoPagamento = metodoPagamento;
            Valor = valor;

            // Status inicial do pagamento
            StatusPagamento = StatusPagamento.Pendente;
            DataPagamento = null;
            CodigoTransacao = null;
        }

        public void GerarCodigoTransacaoLocal()
        {
            if (CodigoTransacao is not null)
                return; // já foi gerado

            var codigo = $"LOCAL-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            DefinirCodigoTransacao(codigo);
        }

        public void DefinirCodigoTransacao(string codigo)
        {
            Guard.AgainstNullOrWhiteSpace(codigo, nameof(codigo));

            Guard.Against<DomainException>(
                CodigoTransacao is not null,
                "O código de transação já foi definido.");

            Guard.Against<DomainException>(
                StatusPagamento != StatusPagamento.Pendente,
                "Não é permitido registrar código após confirmação ou recusa do pagamento.");

            // gerado apenas uma vez, quando o pagamento é aprovado
            CodigoTransacao = codigo;
            SetDataAtualizacao();
        }

        public void ConfirmarPagamento()
        {
            Guard.Against<DomainException>(
                StatusPagamento != StatusPagamento.Pendente,
                "Apenas pagamentos pendentes podem ser confirmados.");

            Guard.AgainstNullOrWhiteSpace(
                CodigoTransacao ?? string.Empty,
                nameof(CodigoTransacao));

            StatusPagamento = StatusPagamento.Aprovado;
            DataPagamento = DateTime.UtcNow;
            SetDataAtualizacao();

            AddDomainEvent(new PagamentoAprovadoEvent(
                Id,
                PedidoId,
                Valor,
                DataPagamento.Value,
                CodigoTransacao));
        }

        public void RecusarPagamento()
        {
            Guard.Against<DomainException>(
                StatusPagamento != StatusPagamento.Pendente,
                "Apenas pagamentos pendentes podem ser recusados.");

            StatusPagamento = StatusPagamento.Recusado;
            DataPagamento = DateTime.UtcNow;
            SetDataAtualizacao();

            AddDomainEvent(new PagamentoRejeitadoEvent(
                Id,
                PedidoId,
                Valor,
                DataPagamento.Value,
                CodigoTransacao));
        }


    }
}
