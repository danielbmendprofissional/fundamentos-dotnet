using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Catalogo.Entities;

namespace Vendas.Domain.Catalogo.Interfaces
{
    /*
        REPOSITÓRIO DE AGGREGATE ROOT (Produto)

        - Produto é Aggregate Root independente.
        - CategoriaId é apenas referência (não navegação).
        - Não carrega Categoria.
        - Não executa regras de negócio.
    */
    public interface IProdutoRepository
    {
        Task<Produto?> ObterPorIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Produto>> ObterPorIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

        Task AdicionarAsync(
            Produto produto,
            CancellationToken cancellationToken = default);

        void Atualizar(Produto produto);
    }
}
