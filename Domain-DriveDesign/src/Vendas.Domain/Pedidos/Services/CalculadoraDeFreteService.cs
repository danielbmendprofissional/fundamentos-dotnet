using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Pedidos.Entities;
using Vendas.Domain.Pedidos.Interfaces;

namespace Vendas.Domain.Pedidos.Services
{
    public sealed class CalculadoraDeFreteService : ICalculadoraDeFreteService
    {
        public decimal CalcularFrete(Pedido pedido)
        {
            if (pedido is null)
                throw new ArgumentNullException(nameof(pedido));

            var cep = pedido.EnderecoEntrega.Cep.Valor;

            // Exemplo de regra simples
            if (cep.StartsWith("30")) // Belo Horizonte
                return 15m;

            if (pedido.ValorTotal >= 500m)
                return 0m;

            return 30m;
        }
    }

}
