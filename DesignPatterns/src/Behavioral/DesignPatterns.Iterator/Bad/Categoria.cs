using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Iterator.Bad
{
    // EXEMPLO OPCIONAL: Representa um nó em uma árvore de categorias de estoque.
    public class Categoria
    {
        public string Nome { get; set; }
        public List<Categoria> SubCategorias { get; set; } = new();
        public List<Produto> Produtos { get; set; } = new();

        public Categoria(string nome) => Nome = nome;
    }
}
