using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Application.IntegrationEvents.Catalogo;
using Vendas.Domain.Catalogo.Interfaces;

namespace Vendas.Infrastructure.EventHandlers.Catalogo
{
    public sealed class PedidoEmSeparacaoIntegrationEventHandler
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PedidoEmSeparacaoIntegrationEventHandler(
            IProdutoRepository produtoRepository,
            IUnitOfWork unitOfWork)
        {
            _produtoRepository = produtoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            PedidoEmSeparacaoIntegrationEvent evt,
            CancellationToken cancellationToken)
        {
            foreach (var produto in evt.Produtos)
            {
                var entity =
                    await _produtoRepository.ObterPorIdAsync(
                        produto.Id,
                        cancellationToken);

                if (entity is null)
                    throw new Exception("Produto não encontrado.");

                entity.AjustarEstoque(-produto.Quantidade, "Reserva para pedido");
            }

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }

}
