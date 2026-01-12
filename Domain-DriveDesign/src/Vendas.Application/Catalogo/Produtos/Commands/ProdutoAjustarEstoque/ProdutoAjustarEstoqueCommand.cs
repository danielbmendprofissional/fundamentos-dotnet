using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Catalogo.Produtos.IntegrationEvents;

namespace Vendas.Application.Catalogo.Produtos.Commands.ProdutoAjustarEstoque
{
    public sealed record ProdutoAjustarEstoqueCommand(
        Guid PedidoId,
        IReadOnlyCollection<ProdutoReservaDTO> Produtos
        ) : IRequest;
}
