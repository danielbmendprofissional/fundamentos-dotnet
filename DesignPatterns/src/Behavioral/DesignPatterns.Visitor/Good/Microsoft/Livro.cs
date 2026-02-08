using DesignPatterns.Visitor.Good.Microsoft.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Visitor.Good.Microsoft
{
    // OBRIGATORIEDADE: Entidades 100% puras. Sem 'Accept', sem interfaces extras.
    // Elas agora são apenas dados (Data Carriers).
    public record Livro(string Titulo, decimal Preco) : IProduto;
}
