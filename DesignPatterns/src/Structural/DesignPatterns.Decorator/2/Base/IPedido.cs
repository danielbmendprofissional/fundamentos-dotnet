using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Decorator._2.Base
{
    public interface IPedido
    {
        decimal CalcularTotal();
        string GetDescricao();
    }
}
