using DesignPatterns.Command.Bad;
using DesignPatterns.Command.Good.Microsoft.Command;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Command.Good.Microsoft.Handler
{
    // OBRIGATORIEDADE: O Handler deve implementar IRequestHandler<TCommand, TResponse>.
    // Esta classe isola a lógica de negócio, facilitando testes unitários.
    public class BaixarEstoqueCommandHandler : IRequestHandler<BaixarEstoqueCommand, bool>
    {
        private readonly EstoqueService _estoque;

        // OBRIGATORIEDADE: Injeção de Dependência via construtor.
        // Preferência usar Interface no DI
        public BaixarEstoqueCommandHandler(EstoqueService estoque)
        {
            _estoque = estoque;
        }

        public Task<bool> Handle(BaixarEstoqueCommand request, CancellationToken cancellationToken)
        {
            // OBRIGATORIEDADE: O processamento ocorre aqui, delegando ao Receiver (Service).
            _estoque.BaixarItem(request.Produto, request.Quantidade);

            return Task.FromResult(true);
        }
    }
}
