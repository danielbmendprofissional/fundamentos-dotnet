using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Command.Good.GOF.Abstractions
{
    // OBRIGATORIEDADE: Seguindo o GOF, deve haver uma interface que define o contrato único.
    // A nomenclatura 'ICommand' é obrigatória para identificar o papel do objeto no padrão.
    public interface ICommand
    {
        // OBRIGATORIEDADE: O método 'Execute' é o AlgorithmInterface que dispara a ação.
        void Execute();

        // EXEMPLO OPCIONAL: Frequentemente usado no GOF para permitir a reversão do estado.
        void Undo();
    }
}
