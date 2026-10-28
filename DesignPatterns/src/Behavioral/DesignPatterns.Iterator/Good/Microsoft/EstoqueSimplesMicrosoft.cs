using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Iterator.Good.Microsoft
{
    public class EstoqueSimplesMicrosoft : IEnumerable<Bad.Produto>
    {
        private readonly Bad.Produto[] _itens =
        {
        new("Monitor UltraWide", 2500),
        new("Teclado Mecânico", 600)
    };

        public IEnumerator<Bad.Produto> GetEnumerator()
        {
            // OBRIGATORIEDADE: O yield percorre o array interno sem expô-lo.
            foreach (var p in _itens) yield return p;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
