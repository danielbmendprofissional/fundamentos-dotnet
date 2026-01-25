using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Pagamento
{
    // Factory Abstract 
    public abstract class PagamentoFactory
    {
        // O Factory Method
        public abstract IPagamento Criar(string tipo);
    }
}
