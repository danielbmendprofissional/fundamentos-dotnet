using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Memento.Good.Microsoft
{
    // OBRIGATORIEDADE: O Originator (Objeto de Negócio).
    public class PerfilUsuario
    {
        // Propriedades privadas para manter o encapsulamento puro.
        private string _nome;
        private string _bio;
        private int _idade;

        public void SetData(string nome, string bio, int idade)
        {
            _nome = nome;
            _bio = bio;
            _idade = idade;
        }

        // OBRIGATORIEDADE: O 'saveState' profissional retorna um Snapshot imutável.
        public PerfilState TakeSnapshot() =>
            new(_nome, _bio, _idade, DateTime.Now);

        // OBRIGATORIEDADE: O 'restore' profissional valida o estado antes de aplicar.
        public void Restore(PerfilState snapshot)
        {
            if (snapshot == null) return;

            _nome = snapshot.Nome;
            _bio = snapshot.Bio;
            _idade = snapshot.Idade;

            Console.WriteLine($"[Microsoft-Way] Restaurado com sucesso: {_nome}");
        }

        public override string ToString() => $"{_nome} | {_bio} | {_idade}";
    }
}
