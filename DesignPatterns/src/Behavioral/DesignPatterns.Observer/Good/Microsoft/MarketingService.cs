using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Observer.Good.Microsoft
{
    public class MarketingService
    {
        // OBRIGATORIEDADE: Injeção de Dependência via Construtor. 
        // Em um ambiente profissional Microsoft, o serviço DEVE receber suas dependências assim.
        public MarketingService(Estoque estoque)
        {
            // OBRIGATORIEDADE: A assinatura do evento (+=) DEVE ocorrer para o padrão funcionar.
            // Sem esta linha, o observador nunca será notificado.
            estoque.OnEstoqueAlterado += NotificarCampanha;
        }

        // EXEMPLO OPCIONAL: O nome do método e a lógica interna (enviar anúncio) são o seu negócio.
        private void NotificarCampanha(string produto, int qtd)
        {
            Console.WriteLine($"MARKETING: Criando anúncios para {produto}.");
        }
    }
}
