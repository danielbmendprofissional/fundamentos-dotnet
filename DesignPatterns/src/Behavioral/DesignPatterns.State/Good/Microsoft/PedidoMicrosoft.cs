using DesignPatterns.State.Good.Microsoft.enums;
using Stateless;
namespace DesignPatterns.State.Good.Microsoft
{
    // OBRIGATORIEDADE: Uso da biblioteca Stateless (padrão de mercado para State)
    // Aqui não há herança obrigando métodos vazios.
    public class PedidoMicrosoft
    {
        private readonly StateMachine<Status, Trigger> _machine;

        public PedidoMicrosoft()
        {
            _machine = new StateMachine<Status, Trigger>(Status.Aberto);

            // OBRIGATORIEDADE: Configuração das transições permitidas.
            // Se não está configurado, a ação simplesmente não existe para aquele estado.
            _machine.Configure(Status.Aberto)
                .Permit(Trigger.Pagar, Status.Pago)
                .Permit(Trigger.Cancelar, Status.Cancelado);

            _machine.Configure(Status.Pago)
                .Permit(Trigger.Cancelar, Status.Cancelado)
                .Ignore(Trigger.Pagar); // OBRIGATORIEDADE: Ignora explicitamente sem precisar de método vazio.

            _machine.Configure(Status.Cancelado)
                .OnEntry(() => Console.WriteLine("[Stateless] Enviando log de pedido perdido..."))
                // OBRIGATORIEDADE: Não configuramos NADA para o Trigger.Pagar aqui.
                // Se tentar pagar um cancelado, a máquina lança uma exceção de transição inválida.
                .Ignore(Trigger.Cancelar);
        }

        public void Pagar() => _machine.Fire(Trigger.Pagar);
        public void Cancelar() => _machine.Fire(Trigger.Cancelar);
    }
}
