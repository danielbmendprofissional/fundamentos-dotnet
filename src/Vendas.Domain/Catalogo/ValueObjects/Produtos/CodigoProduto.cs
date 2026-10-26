using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Catalogo.ValueObjects.Produtos
{
    public sealed class CodigoProduto : ValueObject
    {
        public string Valor { get; }

        private CodigoProduto(string valor)
        {
            Guard.AgainstNullOrWhiteSpace(valor, nameof(valor));

            Guard.Against<DomainException>(valor.Length < 3,
                "O código do produto deve ter ao menos 3 caracteres.");

            Valor = valor.Trim().ToUpper();
        }

        public static CodigoProduto Create(string valor)
            => new CodigoProduto(valor);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Valor;
        }
    }

}
