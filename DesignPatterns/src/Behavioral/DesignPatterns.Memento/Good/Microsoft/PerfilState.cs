using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Memento.Good.Microsoft
{
    // OBRIGATORIEDADE: O estado é um Record (Imutável). 
    // Isso garante que o Memento não sofra alterações externas após salvo.
    public record PerfilState(string Nome, string Bio, int Idade, DateTime SnapshotDate);
}
