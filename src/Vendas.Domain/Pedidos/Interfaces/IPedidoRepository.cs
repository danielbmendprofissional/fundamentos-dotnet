using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Pedidos.Entities;

namespace Vendas.Domain.Pedidos.Interfaces
{
    public interface IPedidoRepository
    {
        Task<Pedido?> ObterPorIdAsync(Guid pedidoId,
            CancellationToken cancellationToken = default);

        Task AdicionarAsync(Pedido pedido, 
            CancellationToken cancellationToken = default);

        void Atualizar(Pedido pedido);

    }
}
