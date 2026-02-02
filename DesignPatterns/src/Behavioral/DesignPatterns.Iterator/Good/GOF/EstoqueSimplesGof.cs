using DesignPatterns.Iterator.Good.GOF.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Iterator.Good.GOF
{
    public class EstoqueSimplesGof : ICollection
    {
        private readonly Bad.Produto[] _itens = { new("Monitor", 1500), new("Teclado", 300) };
        public IIterator CreateIterator() => new EstoqueSimplesIterator(_itens);
    }
}
