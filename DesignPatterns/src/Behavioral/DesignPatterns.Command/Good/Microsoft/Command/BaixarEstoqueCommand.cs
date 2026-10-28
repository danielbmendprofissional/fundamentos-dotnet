using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Command.Good.Microsoft.Command
{
    // OBRIGATORIEDADE: O Command deve implementar IRequest. 
    // Isso o marca como um objeto que será despachado pelo MediatR.
    // Usamos 'record' para garantir imutabilidade, prática recomendada por MVPs.
    public sealed record BaixarEstoqueCommand(string Produto, int Quantidade) : IRequest<bool>;
}
