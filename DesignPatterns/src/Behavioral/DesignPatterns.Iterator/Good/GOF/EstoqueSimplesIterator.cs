using DesignPatterns.Iterator.Good.GOF.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Iterator.Good.GOF
{
    // Solução problema 1
    public class EstoqueSimplesIterator : IIterator
    {
        private readonly Bad.Produto[] _itens;
        private int _posicao = 0;

        public EstoqueSimplesIterator(Bad.Produto[] itens) => _itens = itens;

        public object First() { _posicao = 0; return _itens[_posicao]; }

        public object Next() => HasNext() ? _itens[_posicao++] : null;

        public bool HasNext() => _posicao < _itens.Length;

        public object CurrentItem() => _itens[_posicao];
    }
}
