using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Observer.Good.Microsoft
{
    // Observable
    public class Estoque
    {
        // OBRIGATORIEDADE: O uso de 'event' é o padrão Microsoft para garantir o encapsulamento.
        // Ele impede que classes externas disparem o evento, permitindo apenas a assinatura (+=).
        // O 'Action<string, int>' define o contrato imutável da mensagem.
        public event Action<string, int>? OnEstoqueAlterado;

        // EXEMPLO OPCIONAL: Propriedades de domínio do nosso cenário de Estoque.
        public string ProdutoNome { get; private set; } = string.Empty;
        public int Quantidade { get; private set; }

        public void AtualizarEstoque(string nome, int qtd)
        {
            ProdutoNome = nome;
            Quantidade = qtd;

            Console.WriteLine($"ESTOQUE (MSFT): {nome} atualizado para {qtd}.");

            // OBRIGATORIEDADE: O método 'Invoke' é o gatilho padrão do .NET.
            // Ele percorre a lista de assinantes (Invocation List) e executa os métodos.
            OnEstoqueAlterado?.Invoke(nome, qtd);
        }
    }
}
