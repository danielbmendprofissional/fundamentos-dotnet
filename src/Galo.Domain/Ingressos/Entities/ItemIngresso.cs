using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;

namespace Galo.Domain.Ingressos.Entities
{
    /// <summary>
    /// Item da compra. Guarda um SNAPSHOT (setor e preço no momento da compra): se o preço do setor
    /// mudar depois, o histórico da compra não muda.
    /// </summary>
    public sealed class ItemIngresso : Entity
    {
        public Guid CompraId { get; private set; }
        public string Setor { get; private set; } = string.Empty;
        public decimal PrecoUnitario { get; private set; }
        public int Quantidade { get; private set; }
        public decimal PorcentagemDesconto { get; private set; }
        public decimal ValorTotal { get; private set; }

        private ItemIngresso() { } // EF Core

        internal ItemIngresso(Guid compraId, string setor, decimal precoUnitario, int quantidade)
        {
            Guard.AgainstEmptyGuid(compraId, nameof(compraId));
            Guard.AgainstNullOrWhiteSpace(setor, nameof(setor));
            Guard.Against<DomainException>(precoUnitario <= 0, "O preço unitário deve ser maior que zero.");
            Guard.Against<DomainException>(quantidade <= 0, "A quantidade deve ser maior que zero.");

            CompraId = compraId;
            Setor = setor.Trim();
            PrecoUnitario = precoUnitario;
            Quantidade = quantidade;
            CalcularValorTotal();
        }

        internal void AdicionarUnidades(int quantidade)
        {
            Guard.Against<DomainException>(quantidade <= 0, "Deve-se adicionar pelo menos uma unidade.");
            Quantidade += quantidade;
            SetDataAtualizacao();
            CalcularValorTotal();
        }

        internal void AplicarDesconto(decimal porcentagem)
        {
            Guard.Against<DomainException>(porcentagem < 0 || porcentagem > 100, "O desconto deve estar entre 0 e 100%.");
            PorcentagemDesconto = porcentagem;
            SetDataAtualizacao();
            CalcularValorTotal();
        }

        private void CalcularValorTotal()
        {
            var bruto = PrecoUnitario * Quantidade;
            ValorTotal = decimal.Round(bruto - (bruto * PorcentagemDesconto / 100m), 2);
        }
    }
}
