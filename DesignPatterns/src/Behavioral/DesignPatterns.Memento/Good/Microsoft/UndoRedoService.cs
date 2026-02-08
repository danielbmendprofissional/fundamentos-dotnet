using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Memento.Good.Microsoft
{
    // OBRIGATORIEDADE: Caretaker genérico. Ele não conhece os campos do Perfil.
    // Ele apenas guarda "Snapshots" do tipo T.
    public class UndoRedoService<T>
    {
        private readonly Stack<T> _history = new();

        public void Save(T state) => _history.Push(state);

        public T? Undo()
        {
            if (_history.Count == 0) return default;
            return _history.Pop();
        }
    }
}
