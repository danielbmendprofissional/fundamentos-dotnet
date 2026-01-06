using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Queries.Commom;
using Vendas.Application.Queries.ObterPedidoDetalhe;

namespace Vendas.Application.Abstractions.Persistence.Queries
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
