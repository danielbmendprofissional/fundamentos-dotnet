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
    public sealed class ImagemProduto : ValueObject
    {
        public string Url { get; }
        public int Ordem { get; }

        private ImagemProduto(string url, int ordem)
        {
            Guard.AgainstNullOrWhiteSpace(url, nameof(url));

            Guard.Against<DomainException>(ordem < 1,
                "A ordem da imagem deve ser ≥ 1.");

            Url = url;
            Ordem = ordem;
        }

        public static ImagemProduto Create(string url, int ordem)
            => new ImagemProduto(url, ordem);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Url;
            yield return Ordem;
        }
    }
}
