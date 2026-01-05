using MediatR;
using Vendas.Application.Abstractions;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Application.Commands.Pedidos.AdicionarItemAoPedido;
using Vendas.Domain.Pedidos.Interfaces;

public sealed class AdicionarItemAoPedidoCommandHandler : IRequestHandler<
        AdicionarItemAoPedidoCommand,
        AdicionarItemAoPedidoResultDTO>
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdicionarItemAoPedidoCommandHandler(
        IPedidoRepository pedidoRepository,
        IUnitOfWork unitOfWork)
    {
        _pedidoRepository = pedidoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AdicionarItemAoPedidoResultDTO> Handle(AdicionarItemAoPedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoRepository
            .ObterPorIdAsync(request.PedidoId, cancellationToken);

        if (pedido is null)
            throw new InvalidOperationException("Pedido não encontrado.");

        // REGRA DE NEGÓCIO → DOMAIN
        pedido.AdicionarItem(
            request.ProdutoId,
            request.NomeProduto,
            request.PrecoUnitario,
            request.Quantidade);

        // FECHAMENTO TRANSACIONAL
        await _unitOfWork.CommitAsync(cancellationToken);

        return new AdicionarItemAoPedidoResultDTO(
            pedido.Id,
            pedido.ValorTotal,
            pedido.StatusPedido.ToString()
        );
    }
}
