using DesignPatterns.ChainOfResponsibility.Good.Microsoft.MediatR.Command;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.ChainOfResponsibility.Good.Microsoft.MediatR
{
    // OBRIGATORIEDADE: O "Diretor" (Terminal Handler). 
    // Ele é o fim da linha. O MediatR só o chama se todos os behaviors permitirem.
    public class DiretorHandler : IRequestHandler<AprovarDescontoCommand, string>
    {
        public Task<string> Handle(AprovarDescontoCommand request, CancellationToken ct)
        {
            return Task.FromResult($"[Terminal] Diretor aprovou R${request.Valor} (Fim da Cadeia).");
        }
    }
}
