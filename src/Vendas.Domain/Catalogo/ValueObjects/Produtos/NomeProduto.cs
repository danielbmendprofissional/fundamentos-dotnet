using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Catalogo.ValueObjects.Produtos
{
    public sealed class NomeProduto : ValueObject
    {
        public string Valor { get; }

        private NomeProduto(string valor)
        {
            Guard.AgainstNullOrWhiteSpace(valor, nameof(valor));

            Guard.Against<DomainException>(valor.Length < 3,
                "O nome do produto deve ter ao menos 3 caracteres.");

            Guard.Against<DomainException>(valor.Length > 150,
                "O nome do produto deve ter no máximo 150 caracteres.");

            Valor = valor.Trim();
        }

        public static NomeProduto Create(string valor)
            => new NomeProduto(valor);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Valor;
        }
    }
}
