using DesignPatterns.ChainOfResponsibility.Good.Microsoft.MediatR.Command;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.ChainOfResponsibility.Good.Microsoft.MediatR
{
    // OBRIGATORIEDADE: Elo da Corrente - Gerente
    public class GerenteBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : AprovarDescontoCommand
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            if (request.Valor < 1000)
            {
                Console.WriteLine("[Pipeline] Gerente resolveu o problema.");
                return (TResponse)(object)"Aprovado pelo Gerente";
            }
            return await next();
        }
    }
}
