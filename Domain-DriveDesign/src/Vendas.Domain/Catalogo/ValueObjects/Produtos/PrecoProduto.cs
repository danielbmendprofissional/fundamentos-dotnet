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
    public sealed class PrecoProduto : ValueObject
    {
        public decimal Valor { get; }

        private PrecoProduto(decimal valor)
        {
            Guard.Against<DomainException>(valor <= 0,
                "O preço do produto deve ser maior que zero.");

            Valor = valor;
        }

        public static PrecoProduto Create(decimal valor)
            => new PrecoProduto(valor);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Valor;
        }
    }
}
