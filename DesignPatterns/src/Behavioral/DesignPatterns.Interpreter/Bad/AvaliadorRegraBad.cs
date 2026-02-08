using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Interpreter.Bad
{
    public class AvaliadorRegraBad
    {
        // Lógica procedural e frágil.
        public bool Validar(string expressao, int valor)
        {
            // Se a regra mudar para "MENOR QUE", o código quebra.
            if (expressao.Contains("MAIOR QUE"))
            {
                var partes = expressao.Split("MAIOR QUE");
                return valor > int.Parse(partes[1]);
            }
            return false;
        }
    }
}
