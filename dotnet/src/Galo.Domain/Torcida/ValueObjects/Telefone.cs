using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;

namespace Galo.Domain.Torcida.ValueObjects
{
    public sealed class Telefone : ValueObject
    {
        public string Numero { get; }

        private Telefone(string numero)
        {
            Guard.AgainstNullOrWhiteSpace(numero, nameof(numero));

            var digits = new string(numero.Where(char.IsDigit).ToArray());

            Guard.Against<DomainException>(digits.Length is < 10 or > 11,
                "Telefone deve conter 10 (fixo) ou 11 dígitos (celular).");

            Numero = digits;
        }

        public static Telefone Create(string numero) => new(numero);

        public override string ToString()
            => Numero.Length == 11
                ? Convert.ToInt64(Numero).ToString(@"\(00\) 00000\-0000")
                : Convert.ToInt64(Numero).ToString(@"\(00\) 0000\-0000");

        protected override IEnumerable<object> GetEqualityComponents() { yield return Numero; }
    }
}
