using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Iterator.Bad
{
    // OBRIGATORIEDADE: No cenário BAD, a classe expõe seu armazenamento interno.
    // Isso impede que mudemos a estrutura depois sem quebrar o cliente.
    public class EstoqueSimplesBad
    {
        // OBRIGATORIEDADE: Expor um Array fixo é um erro clássico de design.
        // O cliente é obrigado a saber lidar com índices e tamanhos fixos.
        public Produto[] Itens = new Produto[3]
        {
            new("Monitor", 1200),
            new("Teclado", 300),
            new("Mouse", 150)
        };
    }
}
