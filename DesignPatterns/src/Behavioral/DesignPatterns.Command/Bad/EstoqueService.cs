using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Command.Bad
{
    public class EstoqueService
    {
        public void BaixarItem(string produto, int qtd)
            => Console.WriteLine($"[ESTOQUE] Retirado {qtd} un de {produto}.");

        public void EstornarItem(string produto, int qtd)
            => Console.WriteLine($"[ESTOQUE-ROLLBACK] Item {produto} devolvido ao inventário.");
    }
}
