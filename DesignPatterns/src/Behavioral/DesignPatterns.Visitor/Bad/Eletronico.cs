using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Visitor.Bad
{
    // Classe de domínio "inchada".
    // Ela sabe coisa demais sobre o mundo externo (PDF, Impostos, JSON).
    public class Eletronico
    {
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }

        // DUPLICAÇÃO DE LÓGICA: O código de PDF se repete ou varia levemente aqui.
        public void GerarRelatorioPDF()
        {
            Console.WriteLine($"[PDF] Renderizando eletrônico {Nome} com tabela técnica.");
        }

        // VIOLAÇÃO OCP: Se a regra de imposto mudar ou surgir um novo tipo de imposto (ex: ISS), 
        // você tem que abrir a classe Livro para alterar.
        public decimal CalcularImposto() => Preco * 0.15m;
    }
}
