using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Memento.Bad
{
    // Classe sem encapsulamento (Anêmica).
    // Para o "Undo" funcionar, tudo teve que virar PUBLIC.
    public class PerfilUsuarioBad
    {
        public string Nome { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public int Idade { get; set; }
        public string TemaHex { get; set; } = string.Empty;

        // VIOLAÇÃO: Qualquer parte do sistema pode alterar o TemaHex sem validar
        // se o código é uma cor válida, pois o Setter é público para o histórico ler.
    }
}
