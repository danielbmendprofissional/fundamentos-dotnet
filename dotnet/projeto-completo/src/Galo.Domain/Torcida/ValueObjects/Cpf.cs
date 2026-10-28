using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;

namespace Galo.Domain.Torcida.ValueObjects
{
    public sealed class Cpf : ValueObject
    {
        public string Numero { get; }

        private Cpf(string numero)
        {
            Guard.AgainstNullOrWhiteSpace(numero, nameof(numero));

            var digits = new string(numero.Where(char.IsDigit).ToArray());

            Guard.Against<DomainException>(digits.Length != 11, "CPF deve conter 11 dígitos.");
            Guard.Against<DomainException>(!CpfValido(digits), "CPF inválido.");

            Numero = digits;
        }

        public static Cpf Create(string numero) => new(numero);

        public override string ToString() => Convert.ToInt64(Numero).ToString(@"000\.000\.000\-00");

        protected override IEnumerable<object> GetEqualityComponents() { yield return Numero; }

        private static bool CpfValido(string cpf)
        {
            // rejeita sequências repetidas (111.111.111-11 passa no cálculo, mas é inválido)
            if (new string(cpf[0], cpf.Length) == cpf) return false;

            int Soma(int length, int weight)
            {
                var sum = 0;
                for (var i = 0; i < length; i++) sum += (cpf[i] - '0') * (weight - i);
                return sum;
            }

            var dv1 = Soma(9, 10) % 11;
            dv1 = dv1 < 2 ? 0 : 11 - dv1;

            var dv2 = Soma(10, 11) % 11;
            dv2 = dv2 < 2 ? 0 : 11 - dv2;

            return cpf[9] - '0' == dv1 && cpf[10] - '0' == dv2;
        }

        /// <summary>Gera um CPF válido (dígitos verificadores corretos) a partir de 9 dígitos base. Usado no seed.</summary>
        public static string GerarValido(long baseNoveDigitos)
        {
            var b = (baseNoveDigitos % 1_000_000_000L).ToString("000000000");
            if (new string(b[0], 9) == b) b = "123456789"; // evita sequências repetidas

            int Dv(string s, int pesoInicial)
            {
                var soma = 0;
                for (var i = 0; i < s.Length; i++) soma += (s[i] - '0') * (pesoInicial - i);
                var r = soma % 11;
                return r < 2 ? 0 : 11 - r;
            }

            var d1 = Dv(b, 10);
            var d2 = Dv(b + d1, 11);
            return $"{b}{d1}{d2}";
        }
    }
}
