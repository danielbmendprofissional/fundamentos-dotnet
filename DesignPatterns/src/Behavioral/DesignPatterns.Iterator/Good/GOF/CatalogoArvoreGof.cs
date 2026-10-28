using DesignPatterns.Iterator.Bad;
using DesignPatterns.Iterator.Good.GOF.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Iterator.Good.GOF
{
    // OBRIGATORIEDADE: Implementa a interface Collection
    public class CatalogoArvoreGof : ICollection
    {
        private readonly Bad.Categoria _raiz;
        public CatalogoArvoreGof(Bad.Categoria raiz) => _raiz = raiz;
        public IIterator CreateIterator() => new CatalogoArvoreIterator(_raiz);
    }
}
