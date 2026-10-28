using MediatR;

namespace Vendas.Application.Pedidos.Commands.Cancelar
{
    public sealed record CancelarPedidoCommand(
        Guid PedidoId,
        string? CodigoMotivo
    ) : IRequest;

}
