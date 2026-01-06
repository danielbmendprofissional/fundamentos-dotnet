using MediatR;

namespace Vendas.Application.Commands.Pedidos.CancelarPedido
{
    public sealed record CancelarPedidoCommand(
        Guid PedidoId,
        string? CodigoMotivo
    ) : IRequest;

}
