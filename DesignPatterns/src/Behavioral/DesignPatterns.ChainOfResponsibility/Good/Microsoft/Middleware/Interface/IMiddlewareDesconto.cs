using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.ChainOfResponsibility.Good.Microsoft.Middleware.Interface
{
    // OBRIGATORIEDADE: Interface de Middleware profissional.
    public interface IMiddlewareDesconto
    {
        // O 'next' é o sucessor na corrente, injetado via parâmetro no Invoke.
        Task InvokeAsync(DescontoContext context, Func<DescontoContext, Task> next);
    }
}
