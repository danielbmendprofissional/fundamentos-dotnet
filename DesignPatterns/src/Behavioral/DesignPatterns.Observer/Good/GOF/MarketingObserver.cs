using DesignPatterns.Observer.Good.GOF.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Observer.Good.GOF
{
    // [ConcreteObserver] no diagrama GoF.
    public class MarketingObserver : IObserver
    {
        public void Update(IObservable subject)
        {
            // O Observer avalia se o Subject é do tipo que ele sabe tratar.
            if (subject is Estoque estoque)
            {
                Console.WriteLine($"MARKETING (GOF): Gerando anúncios para {estoque.ProdutoNome}.");
            }
        }
    }
}
