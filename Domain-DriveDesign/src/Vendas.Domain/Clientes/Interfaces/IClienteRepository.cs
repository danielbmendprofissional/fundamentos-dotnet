using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Clientes.Entities;

namespace Vendas.Domain.Clientes.Interfaces
{
    public interface IClienteRepository
    {
        Task<Cliente?> ObterPorIdAsync(Guid pedidoId,
            CancellationToken cancellationToken = default);

        Task AdicionarAsync(Cliente pedido,
            CancellationToken cancellationToken = default);

        void Atualizar(Cliente pedido);
    }
}
