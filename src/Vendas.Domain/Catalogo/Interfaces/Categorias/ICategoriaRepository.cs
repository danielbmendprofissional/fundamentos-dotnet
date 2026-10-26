using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Catalogo.Entities;

namespace Vendas.Domain.Catalogo.Interfaces.Categorias
{
    /*
    REPOSITÓRIO DE AGGREGATE ROOT (Categoria)

    - Responsável apenas por persistir e recuperar Categoria.
    - Não expõe query rica.
    - Não expõe EF.
    - Não coordena transação.
    */

    public interface ICategoriaRepository
    {
        Task<Categoria?> ObterPorIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task AdicionarAsync(
            Categoria categoria,
            CancellationToken cancellationToken = default);

        void Atualizar(Categoria categoria);
    }
}
