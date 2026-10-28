using DesignPatterns.ChainOfResponsibility.Good.Microsoft.Middleware.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.ChainOfResponsibility.Good.Microsoft.Middleware
{
    public class GerenteMiddleware : IMiddlewareDesconto
    {
        public async Task InvokeAsync(DescontoContext context, Func<DescontoContext, Task> next)
        {
            if (context.Valor < 1000)
            {
                Console.WriteLine("[Middleware] Gerente aprovou via Pipeline.");
                return; // Interrompe a corrente (Short-circuit)
            }
            await next(context); // Passa para o próximo
        }
    }
}
