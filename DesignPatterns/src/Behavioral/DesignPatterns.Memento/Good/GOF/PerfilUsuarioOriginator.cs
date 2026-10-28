using DesignPatterns.Memento.Good.GOF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Memento.Good.GOF
{
    // OBRIGATORIEDADE: O Originator (Dono do Estado).
    public class PerfilUsuarioOriginator
    {
        private string _nome;
        private string _bio;
        private int _idade;

        public void Atualizar(string nome, string bio, int idade)
        {
            _nome = nome;
            _bio = bio;
            _idade = idade;
        }

        // OBRIGATORIEDADE: saveState() do seu diagrama.
        public IMemento CriarSnapshot() => new PerfilMemento(_nome, _bio, _idade);

        // OBRIGATORIEDADE: restore(m: Memento) do seu diagrama.
        public void Restaurar(IMemento memento)
        {
            if (memento is not PerfilMemento m) return;

            var (nome, bio, idade) = m.GetSavedState();
            _nome = nome;
            _bio = bio;
            _idade = idade;

            Console.WriteLine($"[Originator] Perfil restaurado: {_nome}");
        }
    }
}
