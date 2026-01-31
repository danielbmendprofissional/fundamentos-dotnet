using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Observer.Bad
{
    public class Estoque
    {
        private readonly MarketingService _marketing = new MarketingService();
        private readonly ComprasService _compras = new ComprasService();

        public void AtualizarEstoque(string nome, int qtd)
        {
            Console.WriteLine($"ESTOQUE: Atualizado para {qtd} unidades de {nome}.");

            // O PROBLEMA ESTÁ AQUI: 
            // Chamadas rígidas. O Estoque controla quem ele avisa.
            _marketing.NotificarCampanha(nome);
            _compras.ValidarReposicao(nome, qtd);
        }
    }
}
