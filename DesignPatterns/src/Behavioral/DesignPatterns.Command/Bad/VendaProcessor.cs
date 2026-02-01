using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Command.Bad
{
    public class VendaProcessor
    {
        private readonly EstoqueService _estoque = new();
        private readonly FinanceiroService _financeiro = new();

        // O PROBLEMA: Este método é um "Script Transaction". 
        // Ele orquestra tudo e, se crescer, vira um monstro impossível de testar.
        public void ExecutarVenda(PedidoDto pedido)
        {
            Console.WriteLine($"--- Iniciando Processamento para: {pedido.Produto} ---");

            try
            {
                // 1. Lógica de negócio misturada com orquestração
                _estoque.BaixarItem(pedido.Produto, pedido.Quantidade);

                // Simulação de erro no meio do processo
                if (pedido.PrecoUnitario <= 0)
                    throw new Exception("Preço inválido para cobrança!");

                _financeiro.ProcessarPagamento(pedido.PrecoUnitario * pedido.Quantidade);

                Console.WriteLine("SUCESSO: Venda concluída.");
            }
            catch (Exception ex)
            {
                // O PIOR DO CENÁRIO BAD: O "Desfazer" manual.
                // Se você tiver 10 passos, seu catch terá que desfazer os 10 um por um.
                // É propenso a esquecimentos e bugs de conciliação.
                Console.WriteLine($"ERRO CRÍTICO: {ex.Message}");
                Console.WriteLine("Tentando desfazer passos anteriores...");

                _estoque.EstornarItem(pedido.Produto, pedido.Quantidade);

                // Note que aqui eu não preciso desfazer o financeiro porque ele nem rodou, 
                // mas em um sistema real com 20 serviços, isso vira um inferno de 'if/else'.
            }
        }
    }
}
