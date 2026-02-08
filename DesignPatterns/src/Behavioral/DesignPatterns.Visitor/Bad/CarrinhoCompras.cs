using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Visitor.Bad
{
    // O CLIENTE (O Caos):
    public class CarrinhoCompras
    {
        public void FecharPedido(List<object> itens)
        {
            foreach (var item in itens)
            {
                // Não há uma interface comum para PDF e Imposto
                // (pois são lógicas de mundos diferentes), o desenvolvedor apela para o 'is'.
                if (item is Livro l)
                {
                    l.GerarRelatorioPDF();
                    Console.WriteLine($"Imposto: {l.CalcularImposto()}");
                }
                else if (item is Eletronico e)
                {
                    e.GerarRelatorioPDF();
                    Console.WriteLine($"Imposto: {e.CalcularImposto()}");
                }
            }
        }
    }
}
