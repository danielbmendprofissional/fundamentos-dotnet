using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Interpreter.Good.Microsoft
{
    public class EvaluatorMicrosoft
    {
        public Func<int, bool> CriarRegraMaiorQue(int limite)
        {
            // OBRIGATORIEDADE: Representação profissional de uma AST (Abstract Syntax Tree).
            ParameterExpression parametro = Expression.Parameter(typeof(int), "valor");
            ConstantExpression constante = Expression.Constant(limite);

            // Montando o nó da árvore (valor > limite)
            BinaryExpression corpo = Expression.GreaterThan(parametro, constante);

            // OBRIGATORIEDADE: Compilando a árvore em um delegado executável (Lambda).
            return Expression.Lambda<Func<int, bool>>(corpo, parametro).Compile();
        }
    }
}
