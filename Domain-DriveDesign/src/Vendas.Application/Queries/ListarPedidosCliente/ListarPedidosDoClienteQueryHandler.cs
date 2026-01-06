using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Abstractions.Persistence.Queries;
using Vendas.Application.Queries.Commom;

namespace Vendas.Application.Queries.ListarPedidos
{
    //Application só conhece ABSTRAÇÕES de infra
    public sealed class ListarPedidosDoClienteQueryHandler
        : IRequestHandler<ListarPedidosDoClienteQuery, IReadOnlyList<PedidoResponseDTO>>
    {
        private readonly IPedidoQueryRepository _repository;

        public ListarPedidosDoClienteQueryHandler(IPedidoQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<PedidoResponseDTO>> Handle(
            ListarPedidosDoClienteQuery request,
            CancellationToken cancellationToken)
        {
            return await _repository.ListarPorClienteAsync(request.ClienteId, cancellationToken);
        }
    }

}
