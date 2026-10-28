using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Observer.Good.Microsoft
{
    public class ComprasService
    {
        // OBRIGATORIEDADE: Injeção de Dependência via Construtor. 
        // Em um ambiente profissional Microsoft, o serviço DEVE receber suas dependências assim.
        public ComprasService(Estoque estoque)
        {
            // OBRIGATORIEDADE: A assinatura do evento (+=) DEVE ocorrer para o padrão funcionar.
            // Sem esta linha, o observador nunca será notificado.
            estoque.OnEstoqueAlterado += AvaliarEstoque;
        }

        // EXEMPLO OPCIONAL: O nome do método e a lógica interna (enviar aviso) são o seu negócio.
        private void AvaliarEstoque(string produto, int qtd)
        {
            if (qtd < 5)
            {
                Console.WriteLine($"COMPRAS: Alerta! {produto} com estoque baixo ({qtd}).");
            }
        }
    }
}
