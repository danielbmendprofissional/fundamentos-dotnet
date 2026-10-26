using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Pedidos.Enums;
using Vendas.Domain.Pedidos.Interfaces;

namespace Vendas.Application.Pedidos.Commands.IniciarPagamento
{
    public sealed class IniciarPagamentoCommandHandler : IRequestHandler<
        IniciarPagamentoCommand,
        IniciarPagamentoCommandResultDTO>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICalculadoraDeFreteService _calculadoraDeFrete;

        public IniciarPagamentoCommandHandler(
            IPedidoRepository pedidoRepository,
            IUnitOfWork unitOfWork,
            ICalculadoraDeFreteService calculadoraDeFrete)
        {
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
            _calculadoraDeFrete = calculadoraDeFrete;
        }

        public async Task<IniciarPagamentoCommandResultDTO> Handle(IniciarPagamentoCommand request, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository
                .ObterPorIdAsync(request.PedidoId, cancellationToken);

            if (pedido is null)
                throw new InvalidOperationException("Pedido não encontrado.");

            // Calcula frete via Domain Service
            var frete = _calculadoraDeFrete.CalcularFrete(pedido);
            pedido.AdicionarCustoFrete(frete);


            // REGRA DE NEGÓCIO → DOMAIN
            var novoPagamentoId = pedido.IniciarPagamento((MetodoPagamento)request.MetodoPagamento);

            // FECHAMENTO TRANSACIONAL
            await _unitOfWork.CommitAsync(cancellationToken);

            return new IniciarPagamentoCommandResultDTO(novoPagamentoId);
        }
    }
}
