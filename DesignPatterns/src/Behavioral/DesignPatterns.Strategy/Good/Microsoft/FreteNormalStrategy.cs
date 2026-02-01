using DesignPatterns.Strategy.Good.Microsoft.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Strategy.Good.Microsoft
{
    public class FreteNormalStrategy : IFreteStrategy
    {
        public double Calcular(double valorPedido)
        {
            return valorPedido * 0.1;
        }
    }
}
