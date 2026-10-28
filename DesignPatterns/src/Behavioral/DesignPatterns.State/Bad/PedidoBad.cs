using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.State.Bad
{
    // OBRIGATORIEDADE: Lógica de negócio soterrada em condicionais.
    public class PedidoBad
    {
        public Status StatusAtual { get; set; }

        public void Cancelar()
        {
            if (StatusAtual == Status.Aberto)
                Console.WriteLine("Pedido cancelado.");
            else if (StatusAtual == Status.Pago)
                Console.WriteLine("Estornando e cancelando...");
            else
                Console.WriteLine("Erro: Pedido já entregue não pode ser cancelado.");
        }
    }
}
