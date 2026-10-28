using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Catalogo.Produtos.Queries.ObterProdutoDetalhes
{
    public sealed record ImagemProdutoResponseDTO(
            string Url,
            int Ordem
        );
}
