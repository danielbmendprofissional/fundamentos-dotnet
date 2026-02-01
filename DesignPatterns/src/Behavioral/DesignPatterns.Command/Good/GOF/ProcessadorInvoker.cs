using DesignPatterns.Command.Good.GOF.Abstractions;

namespace DesignPatterns.Command.Good.GOF
{
    public class ProcessadorInvoker
    {
        // EXEMPLO OPCIONAL: Uso de Stack para gerenciar o histórico de comandos para o Undo.
        private readonly Stack<ICommand> _historico = new();

        public void Processar(ICommand comando)
        {
            comando.Execute();
            _historico.Push(comando);
        }

        public void DesfazerUltimo()
        {
            if (_historico.Count > 0)
            {
                var comando = _historico.Pop();
                comando.Undo();
            }
        }
    }
}
