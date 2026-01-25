using DesignPatterns.Factory.Solution.pure.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Factory.Solution.pure
{
    // Creator Abstract 
    public abstract class PagamentoCreator
    {
        // O Factory Method
        public abstract IPagamento CriarMetodo();

        // Lógica que utiliza o produto criado
        public void FinalizarVenda(decimal valor)
        {
            var metodo = CriarMetodo();
            metodo.Processar(valor);
        }
    }
}
