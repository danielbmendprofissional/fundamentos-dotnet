using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Pedidos.Interfaces;
using Vendas.Application.Pedidos.Queries.Commom;
using Vendas.Application.Pedidos.Queries.ObterPedidoDetalhe;
using Vendas.Infrastructure.Common.Persistence;

namespace Vendas.Infrastructure.Pedidos.Repositories
{
    /*
     Repository de Query não pode retornar Model, pois Model nesse contexto fica em Infrastructure e Application não 
     deve conhecer nada de infrastructure além de ABSTRAÇÕES
    */
    public sealed class PedidoQueryRepository
        : IPedidoQueryRepository
    {
        private readonly VendasQueryDbContext _context;

        public PedidoQueryRepository(
            VendasQueryDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PedidoResponseDTO>> ListarPorClienteAsync(
                Guid clienteId,
                CancellationToken cancellationToken)
        {
            return await _context.PedidoResumo
                .AsNoTracking()
                .Where(p => p.ClienteId == clienteId)
                .OrderByDescending(p => p.DataCriacao)
                .Select(p => new PedidoResponseDTO(
                    p.Id,
                    p.NumeroPedido,
                    p.ValorTotal,
                    p.Status,
                    p.DataCriacao))
                .ToListAsync(cancellationToken);
        }

        public async Task<PedidoDetalheResponseDTO?> ObterDetalheAsync(
            Guid id,
            Guid clienteId,
            CancellationToken cancellationToken)
        {
            return await _context.PedidoDetalhes
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Where(p => p.ClienteId == clienteId)
                .Select(p => new PedidoDetalheResponseDTO(
                    p.Id,
                    p.ClienteId,
                    p.NumeroPedido,
                    p.ValorTotal,
                    p.Status,
                    p.DataCriacao,
                    p.Cep,
                    p.Cidade,
                    p.Estado))
                .SingleOrDefaultAsync(cancellationToken);
        }
    }

}
