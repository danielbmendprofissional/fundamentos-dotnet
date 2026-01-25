using DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Pagamento.USA.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Pagamento.USA
{
    public class USAPagamentoFactory : PagamentoFactory
    {
        //Can use Enums
        private readonly Dictionary<string, Func<IPagamento>> _catalogo = new() {
        { "paypal", () => new Paypal() },
        { "credit", () => new CreditCard() }
    };

        public override IPagamento Criar(string tipo)
        {
            if (_catalogo.TryGetValue(tipo.ToLower(), out var instancia))
                return instancia();

            throw new Exception("Payment method not supported.");
        }
    }
}
