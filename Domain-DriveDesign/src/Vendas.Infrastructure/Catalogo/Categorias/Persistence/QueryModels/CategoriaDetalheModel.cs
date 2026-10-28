using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Infrastructure.Catalogo.Categorias.Persistence.QueryModels
{
    public sealed class CategoriaDetalheModel
    {
        public Guid Id { get; }
        public string Nome { get; } = string.Empty;
        public string Descricao { get; } = string.Empty;
        public bool Ativo { get; }
    }
}
