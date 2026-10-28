using DesignPatterns.Memento.Good.GOF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Memento.Good.GOF
{
    // OBRIGATORIEDADE: O Memento é PRIVADO ou OPACO.
    // Ele guarda o estado, mas ninguém além do Perfil consegue ler os campos.
    public class PerfilMemento : IMemento
    {
        private readonly string _nome;
        private readonly string _bio;
        private readonly int _idade;
        private readonly DateTime _data;

        public PerfilMemento(string nome, string bio, int idade)
        {
            _nome = nome;
            _bio = bio;
            _idade = idade;
            _data = DateTime.Now;
        }

        // Métodos internos que só o Originator usará via Cast.
        public (string, string, int) GetSavedState() => (_nome, _bio, _idade);
        public string GetName() => $"Backup em {_data}";
        public DateTime GetDate() => _data;
    }
}
