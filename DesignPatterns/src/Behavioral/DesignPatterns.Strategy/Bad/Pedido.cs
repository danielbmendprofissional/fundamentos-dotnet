using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Strategy.Bad
{
    public class Pedido
    {
        public double ValorProduto { get; set; }

        public double CalcularTotalComFrete(TipoFrete tipo)
        {
            double frete = 0;

            // O PROBLEMA: Acúmulo de responsabilidade e violação do OCP (Open-Closed Principle).
            // Esta classe "sabe" calcular todos os tipos de frete do sistema.
            if (tipo == TipoFrete.Normal)
            {
                frete = ValorProduto * 0.1;
            }
            else if (tipo == TipoFrete.Expresso)
            {
                frete = (ValorProduto * 0.2) + 10; 
            }
            else if (tipo == TipoFrete.Internacional)
            {
                frete = (ValorProduto * 0.5) + 50; 
            }

            return ValorProduto + frete;
        }
    }
}
