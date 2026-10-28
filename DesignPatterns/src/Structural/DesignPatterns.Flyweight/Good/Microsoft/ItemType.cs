using DesignPatterns.Flyweight.Good.Microsoft.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Flyweight.Good.Microsoft
{
    // A implementação concreta que guarda os bytes pesados na memória.
    public class ItemType : IItemFlyweight
    {
        // Propriedades imutáveis: uma vez criado, o "DNA" do item não muda.
        public byte[] IconData { get; }
        public byte[] Model3D { get; }

        public ItemType(byte[] iconData, byte[] model3D)
        {
            IconData = iconData;
            Model3D = model3D;
        }
    }
}
