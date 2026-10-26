using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;
using Galo.Domain.Ingressos.Enums;

namespace Galo.Domain.Ingressos.Entities
{
    public sealed class PagamentoIngresso : Entity
    {
        public Guid CompraId { get; private set; }
        public MetodoPagamento Metodo { get; private set; }
        public StatusPagamento Status { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime? DataPagamento { get; private set; }
        public string? CodigoTransacao { get; private set; }

        private PagamentoIngresso() { } // EF Core

        internal PagamentoIngresso(Guid compraId, MetodoPagamento metodo, decimal valor)
        {
            Guard.AgainstEmptyGuid(compraId, nameof(compraId));
            Guard.AgainstInvalidEnum(metodo, nameof(metodo));
            Guard.Against<DomainException>(valor <= 0, "O valor do pagamento deve ser maior que zero.");

            CompraId = compraId;
            Metodo = metodo;
            Valor = valor;
            Status = StatusPagamento.Pendente;
        }

        internal void DefinirCodigoTransacao(string? codigo)
        {
            Guard.Against<DomainException>(CodigoTransacao is not null, "O código de transação já foi definido.");
            Guard.Against<DomainException>(Status != StatusPagamento.Pendente,
                "Não é permitido registrar código após confirmação ou recusa do pagamento.");

            CodigoTransacao = string.IsNullOrWhiteSpace(codigo)
                ? $"LOCAL-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}" // gerado localmente quando o gateway não informa
                : codigo.Trim();
            SetDataAtualizacao();
        }

        internal void Confirmar()
        {
            Guard.Against<DomainException>(Status != StatusPagamento.Pendente, "Apenas pagamentos pendentes podem ser confirmados.");
            Guard.AgainstNullOrWhiteSpace(CodigoTransacao, nameof(CodigoTransacao));

            Status = StatusPagamento.Aprovado;
            DataPagamento = DateTime.UtcNow;
            SetDataAtualizacao();
        }

        internal void Recusar()
        {
            Guard.Against<DomainException>(Status != StatusPagamento.Pendente, "Apenas pagamentos pendentes podem ser recusados.");

            Status = StatusPagamento.Recusado;
            DataPagamento = DateTime.UtcNow;
            SetDataAtualizacao();
        }
    }
}
