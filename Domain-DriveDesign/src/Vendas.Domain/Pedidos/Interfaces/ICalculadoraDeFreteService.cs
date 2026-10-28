using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Domain.Pedidos.Interfaces
{
    public interface ICalculadoraDeFreteService
    {
        decimal CalcularFrete(Entities.Pedido pedido);
    }

}
