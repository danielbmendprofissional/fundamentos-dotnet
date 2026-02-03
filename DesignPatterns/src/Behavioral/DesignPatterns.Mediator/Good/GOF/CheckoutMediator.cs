using DesignPatterns.Mediator.Good.GOF.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Mediator.Good.GOF
{
    public class CheckoutMediator : IMediator
    {
        private Carrinho _carrinho;
        private Estoque _estoque;
        private Financeiro _financeiro;

        // OBRIGATORIEDADE: O Mediador conhece todos os componentes para orquestrá-los.
        public void SetComponents(Carrinho c, Estoque e, Financeiro f)
        {
            _carrinho = c;
            _estoque = e;
            _financeiro = f;
        }

        public void Notify(object sender, string evento)
        {
            // OBRIGATORIEDADE: O Mediador decide o fluxo baseado em quem notificou.
            // BOMBA!!! é só o exemplo purista mesmo, mas ta ruim vamos ao Microsoft.
            if (evento == "CheckoutIniciado")
            {
                _estoque.Validar();
            }
            else if (evento == "EstoqueValidado")
            {
                _financeiro.Cobrar();
            }
            else if (evento == "PagamentoConcluido")
            {
                Console.WriteLine("[Mediador] Processo completo. Avisando cliente.");
            }
        }
    }
}
