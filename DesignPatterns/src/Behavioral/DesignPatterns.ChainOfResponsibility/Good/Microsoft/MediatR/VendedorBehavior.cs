using DesignPatterns.ChainOfResponsibility.Good.Microsoft.MediatR.Command;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.ChainOfResponsibility.Good.Microsoft.MediatR
{
    // OBRIGATORIEDADE: Elo da Corrente - Vendedor
    public class VendedorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : AprovarDescontoCommand
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            if (request.Valor < 100)
            {
                Console.WriteLine("[Pipeline] Vendedor resolveu o problema.");
                return (TResponse)(object)"Aprovado pelo Vendedor"; // Short-circuit
            }

            // OBRIGATORIEDADE: O 'next' é o sucessor (Gerente ou Diretor)
            return await next();
        }
    }
}
