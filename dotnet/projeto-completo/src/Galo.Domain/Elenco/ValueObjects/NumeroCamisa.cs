using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;

namespace Galo.Domain.Elenco.ValueObjects
{
    public sealed class NumeroCamisa : ValueObject
    {
        public int Valor { get; }

        private NumeroCamisa(int valor)
        {
            Guard.Against<DomainException>(valor is < 1 or > 99, "A camisa deve estar entre 1 e 99.");
            Valor = valor;
        }

        public static NumeroCamisa Create(int valor) => new(valor);

        public override string ToString() => Valor.ToString();
        protected override IEnumerable<object> GetEqualityComponents() { yield return Valor; }
    }
}
