using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Clientes.Interfaces;
using Vendas.Application.Clientes.Queries.ObterEnderecoPrincipal;
using Vendas.Application.Clientes.Queries.ObterEnderecos;
using Vendas.Domain.Clientes.Entities;
using Vendas.Infrastructure.Common.Persistence;

namespace Vendas.Infrastructure.Clientes.Repositories
{
    public sealed class ClienteQueryRepository
        : IClienteQueryRepository
    {
        private readonly VendasQueryDbContext _context;

        public ClienteQueryRepository(
            VendasQueryDbContext context)
        {
            _context = context;
        }

        public async Task<EnderecoPrincipalReponseDTO?> ObterEnderecoPrincipal(
            Guid clienteId,
            CancellationToken cancellationToken)
        {
            return await (from c in _context.ClientesIDs
                          join e in _context.EnderecoPrincipal on c.EnderecoPrincipalId equals e.Id
                          where c.Id == clienteId
                          select new EnderecoPrincipalReponseDTO(
                              e.ClienteId,
                              e.Id,
                              e.Cep,
                              e.Logradouro,
                              e.Bairro,
                              e.Cidade,
                              e.Estado,
                              e.Pais,
                              e.Complemento
                          ))
                          .AsNoTracking()
                          .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<EnderecoItemReponseDTO>> ObterListaEnderecos(
            Guid clienteId,
            CancellationToken cancellationToken)
        {
            return await _context.EnderecoPrincipal
                .AsNoTracking()
                .Where(e => e.ClienteId == clienteId)
                .Select(e => new EnderecoItemReponseDTO(
                    e.Id,
                    e.ClienteId,
                    e.Cidade,
                    e.Logradouro
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
