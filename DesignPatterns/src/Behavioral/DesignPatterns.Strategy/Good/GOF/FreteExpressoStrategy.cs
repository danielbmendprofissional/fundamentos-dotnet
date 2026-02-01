using DesignPatterns.Strategy.Good.GOF.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Strategy.Good.GOF
{
    // [StrategyB]
    public class FreteExpressoStrategy : IFreteStrategy
    {
        public double Calcular(double valorPedido)
        {
            return valorPedido * 0.2 + 10;
        }
    }
}
