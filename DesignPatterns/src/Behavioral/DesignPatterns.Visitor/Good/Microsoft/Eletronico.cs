using DesignPatterns.Visitor.Good.Microsoft.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Visitor.Good.Microsoft
{
    // OBRIGATORIEDADE: Entidades 100% puras. Sem 'Accept'.
    // Elas agora são apenas dados (Data Carriers).
    public record Eletronico(string Nome, decimal Preco) : IProduto;
}
