using DesignPatterns.Observer.Good.GOF.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Observer.Good.GOF
{
    // [ConcreteObserver] no diagrama GoF.
    public class ComprasObserver : IObserver
    {
        public void Update(IObservable subject)
        {
            // O estado extrínseco (quantidade) é lido diretamente do Subject.
            if (subject is Estoque estoque && estoque.Quantidade < 5)
            {
                Console.WriteLine($"COMPRAS (GOF): Alerta de estoque baixo para {estoque.ProdutoNome}!");
            }
        }
    }
}
