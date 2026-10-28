using DesignPatterns.State.Good.GOF.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.State.Good.GOF
{
    // OBRIGATORIEDADE: O Contexto que troca de estado internamente.
    public class PedidoContext
    {
        private PedidoState _state;

        public PedidoContext(PedidoState state)
        {
            TransitionTo(state);
        }

        public void TransitionTo(PedidoState state)
        {
            Console.WriteLine($"[Context] Transição para {state.GetType().Name}.");
            _state = state;
            _state.SetContext(this);
        }

        // OBRIGATORIEDADE: Delegando a execução para o estado atual (state.handle() na imagem).
        public void Pagar() => _state.Pagar();
        public void Cancelar() => _state.Cancelar();
    }
}
