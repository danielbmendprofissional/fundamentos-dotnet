using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Application.Commands.Pedidos.CriarPedido
{
    public sealed class CriarPedidoCommand
    {
        public Guid ClienteId { get; }
        public EnderecoEntrega EnderecoEntrega { get; }

        public CriarPedidoCommand(
            Guid clienteId,
            EnderecoEntrega enderecoEntrega)
        {
            ClienteId = clienteId;
            EnderecoEntrega = enderecoEntrega;
        }
    }

}
