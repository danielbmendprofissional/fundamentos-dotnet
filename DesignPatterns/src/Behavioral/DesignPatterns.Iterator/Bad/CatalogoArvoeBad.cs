using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Iterator.Bad
{
    // OBRIGATORIEDADE: A classe Categoria não provê meios de navegação. 
    // O cliente é obrigado a conhecer a hierarquia para extrair dados.
    public class CatalogoArvoreBad
    {
        public Categoria Raiz { get; private set; }

        public CatalogoArvoreBad()
        {
            Raiz = new Categoria("Loja");
            var hardware = new Categoria("Hardware");
            hardware.Produtos.Add(new Produto("CPU", 2000));
            Raiz.SubCategorias.Add(hardware);
        }
    }
}
