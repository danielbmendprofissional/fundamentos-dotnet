using Microsoft.EntityFrameworkCore;
using Vendas.Domain.Clientes.Entities;
using Vendas.Domain.Clientes.Interfaces;
using Vendas.Infrastructure.Common.Persistence;

namespace Vendas.Infrastructure.Clientes.Repositories
{
    internal sealed class ClienteRepository : IClienteRepository
    {
        private readonly VendasDbContext _context;

        public ClienteRepository(VendasDbContext context)
        {
            _context = context;
        }

        public async Task<Cliente?> ObterPorIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task AdicionarAsync(
            Cliente cliente,
            CancellationToken cancellationToken = default)
        {
            await _context.Clientes.AddAsync(cliente, cancellationToken);
        }

        public void Atualizar(Cliente cliente)
        {
            var entry = _context.Entry(cliente);
            if (entry.State == EntityState.Detached)
            {
                _context.Clientes.Update(cliente);
            }
        }
    }
}
