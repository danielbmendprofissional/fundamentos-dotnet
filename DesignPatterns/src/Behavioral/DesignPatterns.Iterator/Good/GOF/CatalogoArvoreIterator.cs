using DesignPatterns.Iterator.Bad;
using DesignPatterns.Iterator.Good.GOF.Abstractions;

namespace DesignPatterns.Iterator.Good.GOF
{
    // SOLUÇÃO PROBLEMA 2: Encapsula a Árvore (Linearização).
    public class CatalogoArvoreIterator : IIterator
    {
        private readonly List<Bad.Produto> _listaLinear = new();
        private int _posicao = 0;

        public CatalogoArvoreIterator(Bad.Categoria raiz)
        {
            // A complexidade da árvore morre aqui dentro para o cliente não ver.
            Linearizar(raiz);
        }

        private void Linearizar(Bad.Categoria cat)
        {
            foreach (var p in cat.Produtos) _listaLinear.Add(p);
            foreach (var sub in cat.SubCategorias) Linearizar(sub);
        }

        public object First() { _posicao = 0; return _listaLinear[_posicao]; }

        public object Next() => HasNext() ? _listaLinear[_posicao++] : null;

        public bool HasNext() => _posicao < _listaLinear.Count;

        public object CurrentItem() => _listaLinear[_posicao];
    }
}
