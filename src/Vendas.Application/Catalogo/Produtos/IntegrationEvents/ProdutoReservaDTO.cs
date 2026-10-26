using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Catalogo.Produtos.IntegrationEvents
{
    public sealed record ProdutoReservaDTO(
        Guid Id,
        int Quantidade
    );

}
