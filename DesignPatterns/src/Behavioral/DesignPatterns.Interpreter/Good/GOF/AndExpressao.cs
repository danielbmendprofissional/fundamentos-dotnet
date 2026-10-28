using DesignPatterns.Interpreter.Good.GOF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Interpreter.Good.GOF
{
    // OBRIGATORIEDADE: NonTerminalExpression (Representa um operador lógico - AND).
    public class AndExpressao : IExpressao
    {
        private readonly IExpressao _esquerda;
        private readonly IExpressao _direita;

        public AndExpressao(IExpressao esquerda, IExpressao direita)
        {
            _esquerda = esquerda;
            _direita = direita;
        }

        public bool Interpretar(Contexto contexto) =>
            _esquerda.Interpretar(contexto) && _direita.Interpretar(contexto);
    }
}
