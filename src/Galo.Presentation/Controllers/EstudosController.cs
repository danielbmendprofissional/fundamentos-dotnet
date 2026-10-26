using Galo.Estudos;
using Galo.Estudos.Jobs;
using Galo.Infrastructure.Seed;
using Microsoft.AspNetCore.Mvc;

namespace Galo.Presentation.Controllers
{
    /// <summary>
    /// ENDPOINTS DO LABORATÓRIO DE ESTUDOS (somente ambiente Development).
    /// Cada GET executa um experimento e devolve as medições lado a lado + observações do que aprender.
    /// Roteiro sugerido no README.md da raiz do projeto.
    ///
    /// ATENÇÃO: os tempos são do SEU computador e do SEU banco local, com uma API em execução (JIT, GC, outras tarefas).
    /// Use para entender ORDENS DE GRANDEZA e comportamento, não como benchmark científico (para isso: BenchmarkDotNet).
    /// </summary>
    [ApiController]
    [Route("api/v1/estudos")]
    public sealed class EstudosController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public EstudosController(IWebHostEnvironment env) => _env = env;

        // Estes endpoints MEDEM e MODIFICAM dados de teste: nunca expostos fora de Development.
        private IActionResult? SomenteDesenvolvimento() => _env.IsDevelopment() ? null : NotFound();

        // ---------------------------------------------------------------- dados de exemplo
        /// <summary>Gera dados de exemplo (clubes, jogadores, torcedores, partidas com gols). Idempotente para a base; o volume é incremental.</summary>
        [HttpPost("seed")]
        public async Task<IActionResult> Seed([FromServices] DadosIniciaisSeeder seeder,
            [FromQuery] int jogadores = 60, [FromQuery] int torcedores = 2000, [FromQuery] int partidasEncerradas = 400,
            [FromQuery] int partidasAgendadas = 12, CancellationToken ct = default)
        {
            if (SomenteDesenvolvimento() is { } negado) return negado;

            var opcoes = new SeedOptions(
                Math.Clamp(jogadores, 0, 2_000), Math.Clamp(torcedores, 0, 200_000),
                Math.Clamp(partidasEncerradas, 0, 100_000), Math.Clamp(partidasAgendadas, 0, 200));
            return Ok(await seeder.ExecutarAsync(opcoes, ct));
        }

        /// <summary>Apaga TODOS os dados (ExecuteDelete em massa). Útil para recomeçar os experimentos.</summary>
        [HttpDelete("seed")]
        public async Task<IActionResult> LimparDados([FromServices] DadosIniciaisSeeder seeder, CancellationToken ct)
        {
            if (SomenteDesenvolvimento() is { } negado) return negado;
            await seeder.LimparAsync(ct);
            return NoContent();
        }

        // ---------------------------------------------------------------- Entity Framework
        /// <summary>Tracking x NoTracking (ChangeTracker, snapshot, identity map).</summary>
        [HttpGet("ef/tracking")]
        public async Task<IActionResult> Tracking([FromServices] EfCoreLab lab, [FromQuery] int quantidade = 2000, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.TrackingAsync(quantidade, ct));

        /// <summary>Lazy x Explicit x Eager x Split x Projeção: o problema N+1 com contagem de comandos SQL.</summary>
        [HttpGet("ef/carregamento")]
        public async Task<IActionResult> Carregamento([FromServices] EfCoreLab lab, [FromQuery] int partidas = 50, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.CarregamentoAsync(partidas, ct));

        /// <summary>Projeção, Count x Any, consultas compiladas.</summary>
        [HttpGet("ef/projecao")]
        public async Task<IActionResult> Projecao([FromServices] EfCoreLab lab, [FromQuery] int quantidade = 2000, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.ProjecaoAsync(quantidade, ct));

        /// <summary>SaveChanges x ExecuteUpdate (com rollback: não altera seus dados).</summary>
        [HttpGet("ef/operacoes-em-massa")]
        public async Task<IActionResult> OperacoesEmMassa([FromServices] EfCoreLab lab, [FromQuery] int quantidade = 500, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.OperacoesEmMassaAsync(quantidade, ct));

        /// <summary>Paginação por OFFSET x KEYSET.</summary>
        [HttpGet("ef/paginacao")]
        public async Task<IActionResult> Paginacao([FromServices] EfCoreLab lab, [FromQuery] int tamanhoPagina = 20, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.PaginacaoAsync(tamanhoPagina, ct));

        // ---------------------------------------------------------------- paralelismo
        /// <summary>I/O-bound: sequencial x Task.WhenAll x Parallel.ForEachAsync (+ o erro do DbContext compartilhado).</summary>
        [HttpGet("paralelismo/io")]
        public async Task<IActionResult> ParalelismoIo([FromServices] ParalelismoLab lab, [FromQuery] int consultas = 40, [FromQuery] int grau = 8, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.IoBoundAsync(consultas, grau, ct));

        /// <summary>CPU-bound: for x Parallel.For x PLINQ.</summary>
        [HttpGet("paralelismo/cpu")]
        public async Task<IActionResult> ParalelismoCpu([FromServices] ParalelismoLab lab, [FromQuery] int limite = 2_000_000, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.CpuBoundAsync(limite, ct));

        /// <summary>Race condition: contador sem proteção x Interlocked x lock.</summary>
        [HttpGet("paralelismo/corrida")]
        public async Task<IActionResult> Corrida([FromServices] ParalelismoLab lab, [FromQuery] int threads = 8, [FromQuery] int incrementos = 500_000, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.CondicaoDeCorridaAsync(threads, incrementos, ct));

        /// <summary>Estado do ThreadPool agora.</summary>
        [HttpGet("paralelismo/threadpool")]
        public IActionResult ThreadPoolInfo([FromServices] ParalelismoLab lab)
            => SomenteDesenvolvimento() ?? Ok(lab.ThreadPoolInfo());

        // ---------------------------------------------------------------- em memória
        /// <summary>Filtrar no banco x filtrar em memória (IQueryable x IEnumerable).</summary>
        [HttpGet("memoria/servidor-vs-cliente")]
        public async Task<IActionResult> ServidorVsCliente([FromServices] MemoriaLab lab, CancellationToken ct)
            => SomenteDesenvolvimento() ?? Ok(await lab.ServidorVsClienteAsync(ct));

        /// <summary>List x HashSet x Dictionary.</summary>
        [HttpGet("memoria/colecoes")]
        public async Task<IActionResult> Colecoes([FromServices] MemoriaLab lab, [FromQuery] int tamanho = 100_000, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.ColecoesAsync(tamanho, ct));

        /// <summary>string +=, StringBuilder, Concat, interpolação (alocações e GC).</summary>
        [HttpGet("memoria/strings")]
        public async Task<IActionResult> Strings([FromServices] MemoriaLab lab, [FromQuery] int iteracoes = 20_000, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.StringsAsync(iteracoes, ct));

        /// <summary>Span, stackalloc, ArrayPool.</summary>
        [HttpGet("memoria/span")]
        public async Task<IActionResult> Span([FromServices] MemoriaLab lab, [FromQuery] int iteracoes = 1_000_000, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.SpanEPoolAsync(iteracoes, ct));

        /// <summary>LINQ x foreach x for/Span.</summary>
        [HttpGet("memoria/linq")]
        public async Task<IActionResult> Linq([FromServices] MemoriaLab lab, [FromQuery] int tamanho = 5_000_000, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.LinqVsLacoAsync(tamanho, ct));

        // ---------------------------------------------------------------- em tempo de execução
        /// <summary>Reflection x delegate x expression compilada x dynamic.</summary>
        [HttpGet("runtime/chamadas")]
        public async Task<IActionResult> Chamadas([FromServices] RuntimeLab lab, [FromQuery] int iteracoes = 2_000_000, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.ChamadasAsync(iteracoes, ct));

        /// <summary>Monta um filtro em runtime (Expression Tree) e mostra o SQL. Ex.: ?campo=Camisa&amp;operador=gt&amp;valor=10</summary>
        [HttpGet("runtime/filtro-dinamico")]
        public async Task<IActionResult> FiltroDinamico([FromServices] RuntimeLab lab, [FromQuery] string campo = "Camisa",
            [FromQuery] string operador = "gt", [FromQuery] string valor = "10", CancellationToken ct = default)
        {
            if (SomenteDesenvolvimento() is { } negado) return negado;
            try { return Ok(await lab.FiltroDinamicoAsync(campo, operador, valor, ct)); }
            catch (ArgumentException ex) { return BadRequest(new ProblemDetails { Title = "Filtro inválido", Detail = ex.Message }); }
        }

        /// <summary>Serialização JSON por reflection x source generator.</summary>
        [HttpGet("runtime/serializacao")]
        public async Task<IActionResult> Serializacao([FromServices] RuntimeLab lab, [FromQuery] int itens = 10_000, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.SerializacaoAsync(itens, ct));

        /// <summary>CLR/GC/JIT agora.</summary>
        [HttpGet("runtime/info")]
        public IActionResult InfoRuntime([FromServices] RuntimeLab lab)
            => SomenteDesenvolvimento() ?? Ok(lab.InfoRuntime());

        // ---------------------------------------------------------------- em processamento
        /// <summary>Produtor/consumidor com Channels: ilimitado x limitado (backpressure).</summary>
        [HttpGet("processamento/channels")]
        public async Task<IActionResult> Channels([FromServices] ProcessamentoLab lab, [FromQuery] int itens = 1000, [FromQuery] int consumidores = 2, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.ChannelsAsync(itens, consumidores, ct));

        /// <summary>ToList x streaming (AsAsyncEnumerable) x GroupBy no banco.</summary>
        [HttpGet("processamento/streaming")]
        public async Task<IActionResult> Streaming([FromServices] ProcessamentoLab lab, CancellationToken ct)
            => SomenteDesenvolvimento() ?? Ok(await lab.StreamingAsync(ct));

        /// <summary>Enfileira um job para processar em segundo plano. Tipos: simulado, torcedores-por-plano, reaquecer-classificacoes.</summary>
        [HttpPost("processamento/jobs")]
        public async Task<IActionResult> EnfileirarJob([FromServices] FilaDeJobs fila, [FromQuery] string tipo = "simulado", [FromQuery] int parametro = 20, CancellationToken ct = default)
        {
            if (SomenteDesenvolvimento() is { } negado) return negado;
            var job = await fila.EnfileirarAsync(tipo, parametro, ct);
            return Accepted($"/api/v1/estudos/processamento/jobs/{job.Id}", new { job.Id, job.Tipo, job.Status });   // 202: "aceito, ainda não concluído"
        }

        [HttpGet("processamento/jobs")]
        public IActionResult ListarJobs([FromServices] FilaDeJobs fila)
            => SomenteDesenvolvimento() ?? Ok(fila.Listar());

        [HttpGet("processamento/jobs/{id:guid}")]
        public IActionResult ObterJob(Guid id, [FromServices] FilaDeJobs fila)
            => SomenteDesenvolvimento() ?? (fila.Obter(id) is { } job ? Ok(job) : NotFound());

        [HttpDelete("processamento/jobs/{id:guid}")]
        public IActionResult CancelarJob(Guid id, [FromServices] FilaDeJobs fila)
            => SomenteDesenvolvimento() ?? (fila.Cancelar(id) ? Accepted() : NotFound());

        // ---------------------------------------------------------------- cache
        /// <summary>Sem cache x L1 x L2 x 2 níveis.</summary>
        [HttpGet("cache/comparacao")]
        public async Task<IActionResult> CacheComparacao([FromServices] CacheLab lab, [FromQuery] int repeticoes = 200, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.ComparacaoAsync(repeticoes, ct));

        /// <summary>Cache stampede: ingênuo x protegido.</summary>
        [HttpGet("cache/stampede")]
        public async Task<IActionResult> CacheStampede([FromServices] CacheLab lab, [FromQuery] int concorrentes = 50, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.StampedeAsync(concorrentes, ct));

        /// <summary>Invalidação por grupo (SET no Redis, sem KEYS/SCAN).</summary>
        [HttpGet("cache/invalidacao")]
        public async Task<IActionResult> CacheInvalidacao([FromServices] CacheLab lab, CancellationToken ct)
            => SomenteDesenvolvimento() ?? Ok(await lab.InvalidacaoAsync(ct));

        // ---------------------------------------------------------------- competição (concorrência de dados)
        /// <summary>5 estratégias para N compradores disputarem K ingressos.</summary>
        [HttpGet("concorrencia/estrategias")]
        public async Task<IActionResult> Estrategias([FromServices] ConcorrenciaLab lab, [FromQuery] int capacidade = 20, [FromQuery] int compradores = 100, [FromQuery] int grau = 16, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.EstrategiasAsync(capacidade, compradores, grau, ct));

        /// <summary>O conflito de concorrência otimista, passo a passo.</summary>
        [HttpGet("concorrencia/conflito")]
        public async Task<IActionResult> Conflito([FromServices] ConcorrenciaLab lab, CancellationToken ct)
            => SomenteDesenvolvimento() ?? Ok(await lab.ConflitoPassoAPassoAsync(ct));

        /// <summary>Fluxo completo com RabbitMQ: compras concorrentes disputando as últimas vagas.</summary>
        [HttpGet("concorrencia/fluxo-completo")]
        public async Task<IActionResult> FluxoCompleto([FromServices] ConcorrenciaLab lab, [FromQuery] int capacidade = 10, [FromQuery] int compradores = 40, CancellationToken ct = default)
            => SomenteDesenvolvimento() ?? Ok(await lab.FluxoCompletoAsync(capacidade, compradores, ct));
    }
}
