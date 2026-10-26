using Galo.Application.Common.Telemetry;
using Galo.Application.Competicoes.Queries;
using Galo.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Galo.Estudos.Jobs
{
    /// <summary>
    /// BackgroundService: roda junto com a API, sem requisição HTTP. Consome a FilaDeJobs um job por vez.
    /// Cada job ganha um ESCOPO de DI próprio (DbContext novo): serviços scoped NÃO podem ser injetados direto
    /// num singleton/hosted service.
    /// </summary>
    public sealed class JobWorker : BackgroundService
    {
        private readonly FilaDeJobs _fila;
        private readonly IServiceScopeFactory _scopes;
        private readonly ILogger<JobWorker> _logger;

        public JobWorker(FilaDeJobs fila, IServiceScopeFactory scopes, ILogger<JobWorker> logger)
        {
            _fila = fila;
            _scopes = scopes;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var job in _fila.Leitor.ReadAllAsync(stoppingToken))
            {
                // cancelar por job OU desligar a aplicação
                using var vinculado = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, job.Cancelamento.Token);
                using var activity = GaloTelemetry.Source.StartActivity($"job {job.Tipo}");

                job.Status = StatusJob.Executando;
                job.IniciadoEm = DateTime.UtcNow;
                try
                {
                    await ExecutarAsync(job, vinculado.Token);
                    job.Status = StatusJob.Concluido;
                }
                catch (OperationCanceledException)
                {
                    job.Status = StatusJob.Cancelado;
                    job.Mensagem = "Cancelado pelo usuário.";
                }
                catch (Exception ex)
                {
                    job.Status = StatusJob.Falhou;
                    job.Mensagem = ex.Message;
                    _logger.LogError(ex, "Job {JobId} ({Tipo}) falhou", job.Id, job.Tipo);
                }
                finally
                {
                    job.ConcluidoEm = DateTime.UtcNow;
                }
            }
        }

        private async Task ExecutarAsync(JobInfo job, CancellationToken ct)
        {
            await using var escopo = _scopes.CreateAsyncScope();

            switch (job.Tipo)
            {
                case "simulado":
                    // job didático: N passos de 200 ms (bom para testar progresso e cancelamento)
                    job.Total = Math.Clamp(job.Parametro <= 0 ? 20 : job.Parametro, 1, 200);
                    for (var i = 1; i <= job.Total; i++)
                    {
                        await Task.Delay(200, ct);
                        job.Progresso = i;
                    }
                    break;

                case "torcedores-por-plano":
                    // Lê TODOS os torcedores em STREAMING (sem carregar tudo na memória) e agrega.
                    var db = escopo.ServiceProvider.GetRequiredService<GaloQueryDbContext>();
                    job.Total = await db.Torcedores.CountAsync(ct);

                    var porPlano = new Dictionary<string, int>();
                    await foreach (var plano in db.Torcedores.Select(t => t.Plano).AsAsyncEnumerable().WithCancellation(ct))
                    {
                        porPlano[plano] = porPlano.GetValueOrDefault(plano) + 1;
                        job.Progresso++;
                    }
                    foreach (var (k, v) in porPlano.OrderBy(p => p.Key)) job.Resultado[k] = v;
                    break;

                case "reaquecer-classificacoes":
                    // Invalida e recalcula a classificação de todos os campeonatos (re-popula o cache em 2 níveis).
                    var sender = escopo.ServiceProvider.GetRequiredService<ISender>();
                    var campeonatos = await sender.Send(new ListarCampeonatosQuery(), ct);
                    job.Total = campeonatos.Count;
                    foreach (var campeonato in campeonatos)
                    {
                        var tabela = await sender.Send(new ObterClassificacaoQuery(campeonato.Id), ct);
                        job.Resultado[campeonato.Nome] = $"{tabela.Count} clubes";
                        job.Progresso++;
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Tipo de job desconhecido: '{job.Tipo}'. Use: simulado, torcedores-por-plano, reaquecer-classificacoes.");
            }
        }
    }
}
