using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Visitor.Bad
{
    // Classe de domínio "inchada".
    // Ela sabe coisa demais sobre o mundo externo (PDF, Impostos, JSON).
    public class Livro
    {
        public string Titulo { get; set; } = string.Empty;
        public decimal Preco { get; set; }

        // VIOLAÇÃO SRP: Por que um Livro sabe se desenhar em PDF?
        public void GerarRelatorioPDF()
        {
            Console.WriteLine($"[PDF] Renderizando livro {Titulo} com layout elegante.");
        }

        // VIOLAÇÃO OCP: Se a regra de imposto mudar ou surgir um novo tipo de imposto (ex: ISS), 
        // você tem que abrir a classe Livro para alterar.
        public decimal CalcularImposto() => Preco * 0.05m;
    }
}
