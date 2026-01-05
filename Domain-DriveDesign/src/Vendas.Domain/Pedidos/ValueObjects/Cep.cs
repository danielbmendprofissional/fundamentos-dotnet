using System.Text.RegularExpressions;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Pedidos.ValueObjects
{
    public sealed class Cep : ValueObject
    {
        public string Valor { get; }

        private Cep(string valor)
        {
            Guard.AgainstNullOrWhiteSpace(valor, nameof(valor));

            var normalizado = valor.Replace("-", "").Trim();

            if (!Regex.IsMatch(normalizado, @"^\d{8}$"))
                throw new DomainException("CEP inválido. Deve conter 8 dígitos.");

            Valor = normalizado;
        }

        public static Cep Criar(string valor)
            => new(valor);

        public override string ToString()
            => $"{Valor[..5]}-{Valor[5..]}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Valor;
        }
    }

}