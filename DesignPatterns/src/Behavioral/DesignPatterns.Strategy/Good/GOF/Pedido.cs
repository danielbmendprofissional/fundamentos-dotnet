using DesignPatterns.Strategy.Good.GOF.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Strategy.Good.GOF
{
    // [Context] no seu diagrama.
    public class Pedido
    {
        // OBRIGATORIEDADE: O Contexto deve possuir uma referência para a interface Strategy.
        // Ele não pode conhecer as classes concretas (Normal ou Expresso).
        private IFreteStrategy _strategy;

        public double ValorProduto { get; set; }

        // OBRIGATORIEDADE: Deve existir uma forma de injetar ou trocar a estratégia.
        // Pode ser via construtor ou via método 'SetStrategy'.
        public Pedido(IFreteStrategy strategy)
        {
            _strategy = strategy;
        }

        public double CalcularTotal()
        {
            // OBRIGATORIEDADE: O Contexto delega a operação para o objeto Strategy.
            // Ele executa o 'AlgorithmInterface()' sem saber qual é a classe real por trás.
            double frete = _strategy.Calcular(ValorProduto);
            return ValorProduto + frete;
        }
    }
}
