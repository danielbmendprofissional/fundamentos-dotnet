using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Catalogo.Categorias.Interfaces;
using Vendas.Application.Catalogo.Categorias.Queries.ObterCategoriaDetalhes;
using Vendas.Application.Catalogo.Categorias.Queries.ObterCategoriaResumo;
using Vendas.Infrastructure.Common.Persistence;

namespace Vendas.Infrastructure.Catalogo.Categorias.Repositories
{
    public sealed class CategoriaQueryRepository
        : ICategoriaQueryRepository
    {
        private readonly VendasQueryDbContext _context;

        public CategoriaQueryRepository(
            VendasQueryDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<CategoriaResponseDTO>> ListarAsync(
                CancellationToken cancellationToken)
        {
            return await _context.CategoriaResumo
                .AsNoTracking()
                .Where(x => x.Ativo)
                .Select(p => new CategoriaResponseDTO(
                    p.Id,
                    p.Nome
                    )
                 ).ToListAsync(cancellationToken);
        }

        public async Task<CategoriaDetalhesResponseDTO?> ObterDetalheAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _context.CategoriaDetalhes
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Where(x => x.Ativo)
                .Select(p => new CategoriaDetalhesResponseDTO(
                    p.Id,
                    p.Nome,
                    p.Descricao,
                    p.Ativo)
                ).SingleOrDefaultAsync(cancellationToken);
        }
    }
}
