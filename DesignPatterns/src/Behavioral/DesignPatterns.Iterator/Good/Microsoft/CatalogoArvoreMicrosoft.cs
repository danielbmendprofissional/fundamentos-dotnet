using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Iterator.Good.Microsoft
{
    public class CatalogoArvoreMicrosoft : IEnumerable<Bad.Produto>
    {
        private readonly Bad.Categoria _raiz;
        public CatalogoArvoreMicrosoft(Bad.Categoria raiz) => _raiz = raiz;

        public IEnumerator<Bad.Produto> GetEnumerator() => Percorrer(_raiz).GetEnumerator();

        private IEnumerable<Bad.Produto> Percorrer(Bad.Categoria cat)
        {
            // OBRIGATORIEDADE: Yield return resolve a recursão de forma "preguiçosa".
            // O item só é processado quando o cliente realmente chega nele na iteração.
            foreach (var p in cat.Produtos) yield return p;

            foreach (var sub in cat.SubCategorias)
                foreach (var p in Percorrer(sub))
                    yield return p;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
