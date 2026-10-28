using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Observer.Bad
{
    public class ComprasService
    {
        public void ValidarReposicao(string produto, int quantidade)
        {
            if (quantidade < 5)
            {
                Console.WriteLine($"COMPRAS: Estoque de {produto} em {quantidade}. Gerando pedido de compra!");
            }
        }
    }
}
