using System.Diagnostics;
using System.Text.Json;
using Galo.Application.Common.Interfaces.Caching;
using Galo.Application.Competicoes.Interfaces;
using Galo.Application.Competicoes.Queries;
using Galo.Estudos.Common;
using Galo.Infrastructure.Caching;
using Galo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Galo.Estudos
{
    /// <summary>
    /// LABORATÓRIO DE CACHE: sem cache x L1 (memória) x L2 (Redis) x 2 níveis; cache stampede; invalidação por grupo.
    /// Cenário real: a CLASSIFICAÇÃO do campeonato (agregação sobre centenas de partidas), lida o tempo todo e que só
    /// muda quando uma partida termina.
    /// </summary>
    public sealed class CacheLab
    {
        private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

        private readonly IServiceScopeFactory _scopes;
        private readonly IDbContextFactory<GaloQueryDbContext> _factory;
        private readonly IMemoryCache _memoria;
        private readonly IDistributedCache _distribuido;
        private readonly ICacheStore _cache;
        private readonly IConnectionMultiplexer _redis;
        private readonly string _prefixo;

        public CacheLab(IServiceScopeFactory scopes, IDbContextFactory<GaloQueryDbContext> factory, IMemoryCache memoria,
            IDistributedCache distribuido, ICacheStore cache, IConnectionMultiplexer redis, IOptions<RedisOptions> redisOptions)
        {
            _scopes = scopes;
            _factory = factory;
            _memoria = memoria;
            _distribuido = distribuido;
            _cache = cache;
            _redis = redis;
            _prefixo = redisOptions.Value.KeyPrefix;
        }

        // ================================================================================================
        // 1) SEM CACHE x L1 x L2 x 2 NÍVEIS
        // ================================================================================================
        public async Task<ExperimentoResultado> ComparacaoAsync(int repeticoes, CancellationToken ct)
        {
            repeticoes = Math.Clamp(repeticoes, 10, 2_000);

            await using var preparo = await _factory.CreateDbContextAsync(ct);
            var campeonatoId = await preparo.Campeonatos.Where(c => c.Tipo == "Brasileirao").Select(c => c.Id).FirstOrDefaultAsync(ct);
            if (campeonatoId == Guid.Empty) throw new InvalidOperationException("Sem dados. Execute POST /api/v1/estudos/seed antes.");

            // a consulta "cara" que queremos evitar repetir
            async Task<IReadOnlyList<ClassificacaoItemDto>> Consultar(CancellationToken t)
            {
                await using var escopo = _scopes.CreateAsyncScope();
                var repo = escopo.ServiceProvider.GetRequiredService<IPartidaQueryRepository>();
                return await repo.ObterClassificacaoAsync(campeonatoId, t);
            }

            var prefixo = $"lab:{Guid.NewGuid():N}:";   // chaves exclusivas desta execução

            var medicoes = new List<Medicao>
            {
                await Medidor.MedirAsync($"1. SEM cache ({repeticoes}x)", "Toda leitura vai ao PostgreSQL",
                    async t =>
                    {
                        var linhas = 0;
                        for (var i = 0; i < repeticoes; i++) linhas = (await Consultar(t)).Count;
                        return new Dictionary<string, object?> { ["repeticoes"] = repeticoes, ["clubesNaTabela"] = linhas };
                    }, ct, aquecer: false),

                await Medidor.MedirAsync($"2. L1 - IMemoryCache ({repeticoes}x)", "1 ida ao banco; as demais leituras vêm da memória do processo",
                    async t =>
                    {
                        var chave = prefixo + "l1";
                        var linhas = 0;
                        for (var i = 0; i < repeticoes; i++)
                            linhas = (await _memoria.GetOrCreateAsync(chave, e => { e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1); return Consultar(t); }))!.Count;
                        _memoria.Remove(chave);
                        return new Dictionary<string, object?> { ["repeticoes"] = repeticoes, ["clubesNaTabela"] = linhas };
                    }, ct, aquecer: false),

                await Medidor.MedirAsync($"3. L2 - Redis/IDistributedCache ({repeticoes}x)", "Cada leitura = ida ao Redis (rede) + desserialização JSON. Compartilhado entre instâncias",
                    async t =>
                    {
                        var chave = prefixo + "l2";
                        var linhas = 0;
                        for (var i = 0; i < repeticoes; i++)
                        {
                            var bytes = await _distribuido.GetAsync(chave, t);
                            IReadOnlyList<ClassificacaoItemDto> valor;
                            if (bytes is null)
                            {
                                valor = await Consultar(t);
                                await _distribuido.SetAsync(chave, JsonSerializer.SerializeToUtf8Bytes(valor, Json),
                                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) }, t);
                            }
                            else valor = JsonSerializer.Deserialize<List<ClassificacaoItemDto>>(bytes, Json)!;
                            linhas = valor.Count;
                        }
                        await _distribuido.RemoveAsync(chave, t);
                        return new Dictionary<string, object?> { ["repeticoes"] = repeticoes, ["clubesNaTabela"] = linhas };
                    }, ct, aquecer: false),

                await Medidor.MedirAsync($"4. 2 NÍVEIS - ICacheStore ({repeticoes}x)", "L1 -> L2 -> banco, com proteção contra stampede e índice de grupo para invalidação",
                    async t =>
                    {
                        var chave = prefixo + "dois-niveis";
                        var linhas = 0;
                        for (var i = 0; i < repeticoes; i++)
                            linhas = (await _cache.GetOrCreateAsync<IReadOnlyList<ClassificacaoItemDto>>(
                                chave, Consultar, TimeSpan.FromMinutes(1), "lab-comparacao", t)).Count;
                        await _cache.InvalidateGroupAsync("lab-comparacao", t);
                        return new Dictionary<string, object?> { ["repeticoes"] = repeticoes, ["clubesNaTabela"] = linhas };
                    }, ct, aquecer: false)
            };

            // custo "puro" de uma leitura de cada nível
            var pings = new List<double>();
            for (var i = 0; i < 20; i++) pings.Add((await _redis.GetDatabase().PingAsync()).TotalMilliseconds);
            _memoria.Set(prefixo + "ref", 1, TimeSpan.FromSeconds(5));
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 100_000; i++) _memoria.TryGetValue(prefixo + "ref", out _);
            sw.Stop();

            medicoes.Add(new Medicao("Referência de latência", "Custo isolado de uma leitura em cada nível", 0, 0, 0, new Dictionary<string, object?>
            {
                ["redisPingMedioMs"] = Math.Round(pings.Average(), 3),
                ["leituraL1Microssegundos"] = Math.Round(sw.Elapsed.TotalMilliseconds * 1000 / 100_000, 3)
            }));

            return new ExperimentoResultado("Sem cache x L1 x L2 x 2 níveis (cache-aside)",
                "IMemoryCache, IDistributedCache (Redis), cache-aside, TTL, serialização",
                medicoes,
                new[]
                {
                    "Cache-aside: a APLICAÇÃO decide - lê o cache; no miss, vai ao banco e preenche. (Alternativas: read-through/write-through ficam no próprio cache.)",
                    "L1 é ordens de grandeza mais rápido que L2 (sem rede e sem serializar), mas cada instância da API tem uma cópia: pode ficar defasada.",
                    "L2 (Redis) é compartilhado: após uma invalidação, todas as instâncias enxergam o dado novo. Custa rede + JSON.",
                    "Nada de cache sem plano de INVALIDAÇÃO. Aqui: TTL (expira sozinho) + invalidação por grupo quando uma partida encerra (evento de domínio).",
                    "Cache não é correção: se o dado errado fica 5 minutos no cache, o usuário vê errado por 5 minutos."
                });
        }

        // ================================================================================================
        // 2) CACHE STAMPEDE
        // ================================================================================================
        public async Task<ExperimentoResultado> StampedeAsync(int concorrentes, CancellationToken ct)
        {
            concorrentes = Math.Clamp(concorrentes, 5, 200);

            async Task<IDictionary<string, object?>?> Executar(bool protegido, CancellationToken t)
            {
                var execucoesDoFactory = 0;
                var chave = $"lab:stampede:{Guid.NewGuid():N}";

                async Task<string> FactoryCaro(CancellationToken token)
                {
                    Interlocked.Increment(ref execucoesDoFactory);
                    await Task.Delay(300, token);          // simula a consulta cara (300 ms)
                    return "classificacao-calculada";
                }

                var relogio = Stopwatch.StartNew();
                var tarefas = Enumerable.Range(0, concorrentes).Select(_ => Task.Run(async () =>
                {
                    if (protegido)
                        return await _cache.GetOrCreateAsync(chave, FactoryCaro, TimeSpan.FromSeconds(30), "lab-stampede", t);

                    // VERSÃO INGÊNUA: "se não tem no cache, calcula e guarda" - sem coordenação entre as requisições.
                    if (_memoria.TryGetValue(chave, out string? valor) && valor is not null) return valor;
                    valor = await FactoryCaro(t);
                    _memoria.Set(chave, valor, TimeSpan.FromSeconds(30));
                    return valor;
                }, t)).ToArray();

                await Task.WhenAll(tarefas);
                relogio.Stop();

                _memoria.Remove(chave);
                await _cache.InvalidateGroupAsync("lab-stampede", t);

                return new Dictionary<string, object?>
                {
                    ["requisicoesSimultaneas"] = concorrentes,
                    ["execucoesDaConsultaCara"] = execucoesDoFactory,
                    ["tempoTotalMs"] = Math.Round(relogio.Elapsed.TotalMilliseconds, 0)
                };
            }

            var medicoes = new List<Medicao>
            {
                await Medidor.MedirAsync("1. Cache-aside INGÊNUO", "Chave fria + N requisições: TODAS acham o cache vazio e TODAS executam a consulta cara",
                    t => Executar(false, t), ct, aquecer: false),

                await Medidor.MedirAsync("2. Cache-aside PROTEGIDO (lock por chave)", "Só 1 executa a consulta; as outras esperam o resultado dele",
                    t => Executar(true, t), ct, aquecer: false)
            };

            return new ExperimentoResultado("Cache stampede (dogpile effect)",
                "Thundering herd, SemaphoreSlim por chave, double-check",
                medicoes,
                new[]
                {
                    "Stampede: quando uma chave popular expira, centenas de requisições sofrem miss AO MESMO TEMPO e derrubam o banco com a mesma consulta.",
                    "Solução usada no TwoLevelCacheStore: um SemaphoreSlim por chave + 'double-check' após conseguir a trava.",
                    "Em múltiplas instâncias a trava em memória só protege cada instância; para proteção global use lock distribuído (RedLock) ou 'probabilistic early expiration'.",
                    "Outras defesas: TTL com jitter (aleatoriedade para as chaves não expirarem juntas) e 'stale-while-revalidate' (serve o antigo enquanto recalcula)."
                });
        }

        // ================================================================================================
        // 3) INVALIDAÇÃO POR GRUPO (sem KEYS/SCAN)
        // ================================================================================================
        public async Task<ExperimentoResultado> InvalidacaoAsync(CancellationToken ct)
        {
            var grupo = "lab-invalidacao";
            var chaves = new[] { $"lab:inv:{Guid.NewGuid():N}:a", $"lab:inv:{Guid.NewGuid():N}:b", $"lab:inv:{Guid.NewGuid():N}:c" };

            foreach (var chave in chaves)
                await _cache.GetOrCreateAsync(chave, _ => Task.FromResult("valor-" + chave[^1]), TimeSpan.FromMinutes(5), grupo, ct);

            var antes = new Dictionary<string, object?>();
            foreach (var chave in chaves) antes[chave[^1].ToString()] = await _distribuido.GetAsync(chave, ct) is not null;

            var db = _redis.GetDatabase();
            var membrosDoIndice = (await db.SetMembersAsync($"{_prefixo}cache-group:{grupo}")).Length;

            await _cache.InvalidateGroupAsync(grupo, ct);

            var depois = new Dictionary<string, object?>();
            foreach (var chave in chaves) depois[chave[^1].ToString()] = await _distribuido.GetAsync(chave, ct) is not null;
            var membrosDepois = (await db.SetMembersAsync($"{_prefixo}cache-group:{grupo}")).Length;

            return new ExperimentoResultado("Invalidação de cache por grupo",
                "Índice de chaves em SET do Redis; por que não usar KEYS/SCAN; pub/sub para o L1",
                new[]
                {
                    new Medicao("Antes de invalidar", "3 chaves gravadas no grupo", 0, 0, 0, new Dictionary<string, object?>
                        { ["existeNoRedis(a,b,c)"] = antes, ["membrosNoIndiceDoGrupo"] = membrosDoIndice }),
                    new Medicao("Depois de invalidar", "InvalidateGroupAsync apaga as chaves e o índice", 0, 0, 0, new Dictionary<string, object?>
                        { ["existeNoRedis(a,b,c)"] = depois, ["membrosNoIndiceDoGrupo"] = membrosDepois })
                },
                new[]
                {
                    "Como NÃO podemos usar KEYS/SCAN (proibidos nesta infra - e perigosos em produção: KEYS bloqueia o Redis inteiro), cada chave é registrada em um SET 'estudos:cache-group:<grupo>'.",
                    "Invalidar = ler os membros do SET, apagar cada chave e apagar o SET. Custo proporcional ao grupo, não ao banco inteiro.",
                    "O L1 das OUTRAS instâncias é limpo por pub/sub (canal 'estudos:cache-invalidate'); o L1 também tem TTL máximo curto como rede de segurança.",
                    "Toda chave/canal usa o prefixo 'estudos:' porque a ACL do usuário só enxerga esse padrão."
                });
        }
    }
}
