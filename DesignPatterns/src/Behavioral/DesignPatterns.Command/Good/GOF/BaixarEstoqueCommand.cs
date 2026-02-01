using DesignPatterns.Command.Bad;
using DesignPatterns.Command.Good.GOF.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Command.Good.GOF
{
    // OBRIGATORIEDADE: Implementa ICommand e encapsula o Receiver (EstoqueService).
    // Note que o comando "sabe" como se executar e como se desfazer usando o serviço.
    public class BaixarEstoqueCommand : ICommand
    {
        private readonly EstoqueService _estoque;
        private readonly string _produto;
        private readonly int _quantidade;

        // OBRIGATORIEDADE: Parâmetros necessários para a execução devem ser injetados no construtor.
        public BaixarEstoqueCommand(EstoqueService estoque, string produto, int quantidade)
        {
            _estoque = estoque;
            _produto = produto;
            _quantidade = quantidade;
        }

        public void Execute() => _estoque.BaixarItem(_produto, _quantidade);

        public void Undo() => _estoque.EstornarItem(_produto, _quantidade);
    }
}
