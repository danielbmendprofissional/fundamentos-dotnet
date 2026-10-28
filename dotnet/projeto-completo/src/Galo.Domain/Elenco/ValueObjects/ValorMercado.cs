using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;

namespace Galo.Domain.Elenco.ValueObjects
{
    /// <summary>Valor de mercado em reais. Zero é permitido (categoria de base).</summary>
    public sealed class ValorMercado : ValueObject
    {
        public decimal Valor { get; }

        private ValorMercado(decimal valor)
        {
            Guard.Against<DomainException>(valor < 0, "O valor de mercado não pode ser negativo.");
            Valor = decimal.Round(valor, 2);
        }

        public static ValorMercado Create(decimal valor) => new(valor);

        public override string ToString() => Valor.ToString("C");
        protected override IEnumerable<object> GetEqualityComponents() { yield return Valor; }
    }
}
