using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Pedidos.Queries.Commom;
using Vendas.Application.Pedidos.Queries.ObterPedidoDetalhe;

namespace Vendas.Application.Pedidos.Interfaces
{
    public interface IPedidoQueryRepository
    {
        Task<IReadOnlyList<PedidoResponseDTO>> ListarPorClienteAsync(
            Guid clienteId,
            CancellationToken cancellationToken);

        Task<PedidoDetalheResponseDTO?> ObterDetalheAsync(
            Guid id,
            Guid clienteId,
            CancellationToken cancellationToken);
    }
}
