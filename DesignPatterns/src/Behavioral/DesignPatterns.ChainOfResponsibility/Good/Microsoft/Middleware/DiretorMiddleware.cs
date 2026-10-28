using DesignPatterns.ChainOfResponsibility.Good.Microsoft.Middleware.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.ChainOfResponsibility.Good.Microsoft.Middleware
{
    // OBRIGATORIEDADE: O Terminal Middleware.
    // Ele é o fim da linha. Não há 'next' aqui porque ele é o executor final.
    public class DiretorMiddleware : IMiddlewareDesconto
    {
        public Task InvokeAsync(DescontoContext context, Func<DescontoContext, Task> next)
        {
            // OBRIGATORIEDADE: Como ele é o Diretor, ele aprova qualquer valor 
            // que chegou até ele sem passar para ninguém.
            Console.WriteLine($"[Terminal] Diretor: Desconto de R${context.Valor} aprovado em última instância.");

            // Note que NÃO chamamos o 'next'. A corrente morre aqui.
            return Task.CompletedTask;
        }
    }
}
