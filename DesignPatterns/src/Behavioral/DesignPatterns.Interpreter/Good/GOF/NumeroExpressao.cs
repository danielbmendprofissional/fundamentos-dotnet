using DesignPatterns.Interpreter.Good.GOF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Interpreter.Good.GOF
{
    // OBRIGATORIEDADE: TerminalExpression (Representa um valor fixo).
    public class NumeroExpressao : IExpressao
    {
        private readonly int _numero;
        public NumeroExpressao(int numero) => _numero = numero;
        public bool Interpretar(Contexto contexto) => contexto.ValorReferencia > _numero;
    }
}
