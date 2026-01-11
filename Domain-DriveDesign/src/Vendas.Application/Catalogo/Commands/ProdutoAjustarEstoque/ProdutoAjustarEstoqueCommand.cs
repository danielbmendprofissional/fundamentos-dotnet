using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Catalogo.IntegrationEvents;

namespace Vendas.Application.Catalogo.Commands.ProdutoAjustarEstoque
{
    public sealed record ProdutoAjustarEstoqueCommand(
        Guid PedidoId,
        IReadOnlyCollection<ProdutoReservaDTO> Produtos
        ) : IRequest;
}
