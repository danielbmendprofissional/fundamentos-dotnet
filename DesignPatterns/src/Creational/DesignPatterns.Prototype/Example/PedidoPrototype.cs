using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Prototype.Example
{
    public class Endereco { public string Rua { get; set; } }

    public class PedidoPrototype : ICloneable
    {
        public Guid Id { get; set; }
        public decimal Valor { get; set; }
        public Endereco EnderecoEntrega { get; set; }

        // .NET native way to implement Prototype
        public object Clone()
        {
            // Shallow Copy
            var clone = (PedidoPrototype)this.MemberwiseClone();

            // Deep Copy
            clone.EnderecoEntrega = new Endereco { Rua = this.EnderecoEntrega.Rua };

            return clone;
        }
    }
}
