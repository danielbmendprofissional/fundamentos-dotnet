using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Flyweight.Good.Microsoft.Interface
{
    // Define o contrato de dados intrínsecos (o que é pesado e compartilhado).
    public interface IItemFlyweight
    {
        byte[] IconData { get; }
        byte[] Model3D { get; }
    }
}
