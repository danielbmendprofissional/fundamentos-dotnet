using Microsoft.Extensions.Logging;

namespace DesignPatterns.TemplateMethod.Good.Microsoft
{
    // OBRIGATORIEDADE: Abordagem profissional usando Injeção de Dependência e Delegates.
    public class ProcessadorPagamentoMicrosoft
    {
        private readonly ILogger<ProcessadorPagamentoMicrosoft> _logger;

        public ProcessadorPagamentoMicrosoft(ILogger<ProcessadorPagamentoMicrosoft> logger)
        {
            _logger = logger;
        }

        // OBRIGATORIEDADE: O "Template" agora é um método que recebe a ação específica.
        public async Task ProcessarAsync(string descricao, Func<Task> operacaoEspecifica)
        {
            // Passo Fixo 1: Log
            _logger.LogInformation($"[MSFT] Iniciando: {descricao}");

            try
            {
                // Passo Fixo 2: Validação Comum
                Console.WriteLine("[MSFT] Validando infraestrutura...");

                // Passo Variável: Inversão de controle via Delegate
                await operacaoEspecifica();

                // Passo Fixo 3: Finalização
                Console.WriteLine("[MSFT] Notificando sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha no processamento.");
                throw;
            }
        }
    }
}
