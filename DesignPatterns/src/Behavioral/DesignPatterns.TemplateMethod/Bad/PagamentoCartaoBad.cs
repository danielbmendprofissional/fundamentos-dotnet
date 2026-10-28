using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.TemplateMethod.Bad
{
    // OBRIGATORIEDADE: Código duplicado. O fluxo (Log -> Validar -> Processar -> Email) 
    // está repetido em todas as classes. Se o fluxo mudar, você tem que mexer em tudo.
    public class PagamentoCartaoBad
    {
        public void Processar()
        {
            Console.WriteLine("[Log] Iniciando...");
            Console.WriteLine("[Validar] Checando limite do cartão...");
            Console.WriteLine("[Cartão] Chamando operadora..."); // Detalhe específico
            Console.WriteLine("[Email] Enviando comprovante...");
        }
    }
}
