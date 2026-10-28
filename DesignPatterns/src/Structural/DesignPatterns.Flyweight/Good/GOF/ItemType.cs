using DesignPatterns.Flyweight.Good.GOF.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Flyweight.Good.GOF
{
    // [ConcreteFlyweight] no diagrama GoF.
    public class ItemType : IItemFlyweight
    {
        // intrinsicState: Estes bytes são compartilhados por milhares de objetos.
        // Eles são privados e imutáveis após a criação.
        private readonly byte[] _iconData;
        private readonly byte[] _model3D;

        public ItemType(byte[] iconData, byte[] model3D)
        {
            _iconData = iconData;
            _model3D = model3D;
        }

        // Implementação da Operação: Aqui o intrínseco (bytes) encontra o extrínseco (parâmetros).
        public void Display(Guid id, string owner, string name, string description)
        {
            // O Flyweight usa seus dados internos para realizar a tarefa solicitada pelo contexto.
            // Ele não "joga fora" os bytes, ele os consome para executar o comportamento esperado.
            Console.WriteLine($"Item {id}: {name} owned by {owner} (Utilizando {_iconData.Length + _model3D.Length} bytes compartilhados)");
        }
    }
}
