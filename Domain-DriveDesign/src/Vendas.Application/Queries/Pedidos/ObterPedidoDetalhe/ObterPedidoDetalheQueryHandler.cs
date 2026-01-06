using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Abstractions.Persistence.Queries;

namespace Vendas.Application.Queries.Pedidos.ObterPedidoDetalhe
{
    public sealed class ObterPedidoDetalheQueryHandler
        : IRequestHandler<
        ObterPedidoDetalheQuery,
        PedidoDetalheResponseDTO>
    {
        private readonly IPedidoQueryRepository _repository;

        public ObterPedidoDetalheQueryHandler(
            IPedidoQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<PedidoDetalheResponseDTO> Handle(ObterPedidoDetalheQuery request, CancellationToken cancellationToken)
        {
            var retorno = await _repository.ObterDetalheAsync(
                request.Id,
                request.ClienteId,
                cancellationToken);

            if (retorno == null)
                throw new NotFoundException("Pedido não encontrado.");

            return retorno;
        }
    }

}
