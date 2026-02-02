using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Iterator.Good.GOF.Abstractions
{
    // OBRIGATORIEDADE: O Iterator DEVE ter esses métodos.
    // Esta interface permite que o cliente navegue sem conhecer a estrutura da coleção.
    public interface IIterator
    {
        object First();       // Volta para o início.
        object Next();        // Move para o próximo e o retorna.
        bool HasNext();       // OBRIGATORIEDADE: Verifica se ainda há itens (conforme imagem).
        object CurrentItem(); // Retorna o item da posição atual.
    }
}
