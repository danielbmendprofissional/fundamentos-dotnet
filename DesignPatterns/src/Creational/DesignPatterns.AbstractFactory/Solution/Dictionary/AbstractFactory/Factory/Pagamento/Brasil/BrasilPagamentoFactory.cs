using DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Pagamento.Brasil.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Pagamento.Brasil
{
    public class BrasilPagamentoFactory : PagamentoFactory
    {
        //Can use Enums
        private readonly Dictionary<string, Func<IPagamento>> _catalogo = new() {
        { "pix", () => new Pix() },
        { "boleto", () => new Boleto() },
        { "credito", () => new Credito() },
        { "debito", () => new Debito() }
    };

        public override IPagamento Criar(string tipo)
        {
            if (_catalogo.TryGetValue(tipo.ToLower(), out var instancia))
                return instancia();

            throw new Exception("Método de pagamento não suportado no Brasil.");
        }
    }
}
