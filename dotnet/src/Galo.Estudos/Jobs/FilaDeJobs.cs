using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Galo.Estudos.Jobs
{
    public enum StatusJob { Enfileirado, Executando, Concluido, Falhou, Cancelado }

    /// <summary>Estado observável de um job em processamento (o que a API devolve em GET /jobs/{id}).</summary>
    public sealed class JobInfo
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Tipo { get; init; } = string.Empty;
        public int Parametro { get; init; }
        public StatusJob Status { get; set; } = StatusJob.Enfileirado;
        public int Progresso { get; set; }
        public int Total { get; set; }
        public string? Mensagem { get; set; }
        public Dictionary<string, object?> Resultado { get; } = new();
        public DateTime EnfileiradoEm { get; init; } = DateTime.UtcNow;
        public DateTime? IniciadoEm { get; set; }
        public DateTime? ConcluidoEm { get; set; }

        internal CancellationTokenSource Cancelamento { get; } = new();
    }

    /// <summary>
    /// FILA EM MEMÓRIA para processamento em SEGUNDO PLANO.
    /// A API responde 202 Accepted na hora e o trabalho demorado roda no JobWorker (BackgroundService).
    ///
    /// System.Threading.Channels = fila assíncrona, thread-safe, com backpressure: o padrão moderno do .NET para
    /// produtor/consumidor (substitui BlockingCollection em código async).
    ///
    /// LIMITAÇÃO (de propósito): tudo vive na memória do processo. Reiniciou a API, perdeu a fila e o histórico.
    /// Em produção: Hangfire/Quartz (banco), ou mensageria (RabbitMQ) para jobs duráveis e distribuídos.
    /// </summary>
    public sealed class FilaDeJobs
    {
        private readonly Channel<JobInfo> _canal = Channel.CreateBounded<JobInfo>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,   // fila cheia => quem produz ESPERA (backpressure), nada se perde
            SingleReader = true
        });

        private readonly ConcurrentDictionary<Guid, JobInfo> _jobs = new();

        public ChannelReader<JobInfo> Leitor => _canal.Reader;

        public async Task<JobInfo> EnfileirarAsync(string tipo, int parametro, CancellationToken ct)
        {
            var job = new JobInfo { Tipo = tipo, Parametro = parametro };
            _jobs[job.Id] = job;
            await _canal.Writer.WriteAsync(job, ct);
            return job;
        }

        public JobInfo? Obter(Guid id) => _jobs.GetValueOrDefault(id);

        public IReadOnlyList<JobInfo> Listar() => _jobs.Values.OrderByDescending(j => j.EnfileiradoEm).Take(50).ToList();

        public bool Cancelar(Guid id)
        {
            if (!_jobs.TryGetValue(id, out var job) || job.Status is StatusJob.Concluido or StatusJob.Falhou or StatusJob.Cancelado) return false;
            job.Cancelamento.Cancel();
            return true;
        }
    }
}
