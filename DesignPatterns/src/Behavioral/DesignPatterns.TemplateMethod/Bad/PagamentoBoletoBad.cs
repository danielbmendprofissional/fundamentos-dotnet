using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.TemplateMethod.Bad
{
    // OBRIGATORIEDADE: Código duplicado. O fluxo (Log -> Validar -> Processar -> Email) 
    // está repetido em todas as classes. Se o fluxo mudar, você tem que mexer em tudo.
    public class PagamentoBoletoBad
    {
        public void Processar()
        {
            Console.WriteLine("[Log] Iniciando...");
            Console.WriteLine("[Validar] Checando dados do boleto...");
            Console.WriteLine("[Boleto] Gerando linha digitável..."); // Detalhe específico
            Console.WriteLine("[Email] Enviando comprovante...");
        }
    }
}
