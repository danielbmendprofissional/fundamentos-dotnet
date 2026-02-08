using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.ChainOfResponsibility.Good.Microsoft.MediatR.Command
{
    // OBRIGATORIEDADE: O Comando (A requisição que passará pela corrente)
    public record AprovarDescontoCommand(decimal Valor) : IRequest<string>;
}
