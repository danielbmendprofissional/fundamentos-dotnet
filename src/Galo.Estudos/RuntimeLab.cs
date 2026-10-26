using System.Linq.Expressions;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Galo.Application.Elenco.Queries;
using Galo.Estudos.Common;
using Galo.Infrastructure.Persistence;
using Galo.Infrastructure.Persistence.QueryModels;
using Microsoft.EntityFrameworkCore;

namespace Galo.Estudos
{
    /// <summary>Contexto de serialização GERADO em tempo de COMPILAÇÃO (source generator do System.Text.Json).</summary>
    [JsonSerializable(typeof(List<JogadorResumoDto>))]
    internal sealed partial class LabJsonContext : JsonSerializerContext { }

    /// <summary>
    /// LABORATÓRIO DE "TEMPO DE EXECUÇÃO" (runtime):
    /// reflection x delegate x expression tree x dynamic; montagem de filtros dinâmicos em runtime (Expression Trees
    /// traduzidas para SQL pelo EF); serialização por reflection x source generator; informações do runtime/GC/JIT.
    /// </summary>
    public sealed class RuntimeLab
    {
        private readonly IDbContextFactory<GaloQueryDbContext> _factory;
        public RuntimeLab(IDbContextFactory<GaloQueryDbContext> factory) => _factory = factory;

        public sealed class Calculadora
        {
            public int Valor { get; set; } = 7;
            public int Somar(int a, int b) => a + b;
        }

        // ================================================================================================
        // 1) FORMAS DE CHAMAR UM MÉTODO
        // ================================================================================================
        public Task<ExperimentoResultado> ChamadasAsync(int iteracoes, CancellationToken ct)
        {
            iteracoes = Math.Clamp(iteracoes, 100_000, 10_000_000);
            var alvo = new Calculadora();

            // Tudo isto é preparado UMA vez (fora da medição): descobrir/compilar é caro; chamar é barato.
            MethodInfo metodo = typeof(Calculadora).GetMethod(nameof(Calculadora.Somar))!;
            PropertyInfo propriedade = typeof(Calculadora).GetProperty(nameof(Calculadora.Valor))!;
            var delegado = (Func<Calculadora, int, int, int>)Delegate.CreateDelegate(typeof(Func<Calculadora, int, int, int>), metodo);

            var pInst = Expression.Parameter(typeof(Calculadora), "c");
            var pA = Expression.Parameter(typeof(int), "a");
            var pB = Expression.Parameter(typeof(int), "b");
            var compilada = Expression.Lambda<Func<Calculadora, int, int, int>>(
                Expression.Call(pInst, metodo, pA, pB), pInst, pA, pB).Compile();

            var getterCompilado = Expression.Lambda<Func<Calculadora, int>>(Expression.Property(pInst, propriedade), pInst).Compile();
            dynamic dinamico = alvo;

            var medicoes = new List<Medicao>
            {
                Medidor.Medir("1. Chamada direta", "O compilador resolve tudo em tempo de compilação (o JIT ainda pode inlinar)", () =>
                {
                    long s = 0; for (var i = 0; i < iteracoes; i++) s += alvo.Somar(i, 1);
                    return new Dictionary<string, object?> { ["soma"] = s };
                }),

                Medidor.Medir("2. Delegate (CreateDelegate)", "Descobre o método UMA vez e chama por delegate: quase tão rápido quanto direto", () =>
                {
                    long s = 0; for (var i = 0; i < iteracoes; i++) s += delegado(alvo, i, 1);
                    return new Dictionary<string, object?> { ["soma"] = s };
                }),

                Medidor.Medir("3. Expression tree compilada", "Constrói código em runtime e compila para IL/delegate (custo inicial alto, chamada rápida)", () =>
                {
                    long s = 0; for (var i = 0; i < iteracoes; i++) s += compilada(alvo, i, 1);
                    return new Dictionary<string, object?> { ["soma"] = s };
                }),

                Medidor.Medir("4. MethodInfo.Invoke (reflection)", "Boxing de argumentos/retorno + verificações a cada chamada", () =>
                {
                    long s = 0; for (var i = 0; i < iteracoes; i++) s += (int)metodo.Invoke(alvo, new object[] { i, 1 })!;
                    return new Dictionary<string, object?> { ["soma"] = s };
                }),

                Medidor.Medir("5. dynamic", "Resolução em runtime pelo DLR (com cache de call site)", () =>
                {
                    long s = 0; for (var i = 0; i < iteracoes; i++) s += (int)dinamico.Somar(i, 1);
                    return new Dictionary<string, object?> { ["soma"] = s };
                }),

                Medidor.Medir("6. PropertyInfo.GetValue x getter compilado", "Leitura de propriedade por reflection x por expression compilada", () =>
                {
                    long a = 0, b = 0;
                    for (var i = 0; i < iteracoes; i++) a += (int)propriedade.GetValue(alvo)!;
                    for (var i = 0; i < iteracoes; i++) b += getterCompilado(alvo);
                    return new Dictionary<string, object?> { ["viaReflection"] = a, ["viaExpression"] = b };
                })
            };

            return Task.FromResult(new ExperimentoResultado("Formas de chamar um método em tempo de execução",
                "Reflection, delegates, expression trees, dynamic",
                medicoes,
                new[]
                {
                    "Reflection é lenta por CHAMADA (boxing + checagens), não por existir. O truque dos frameworks (EF, AutoMapper, serializers, DI): descobrir UMA vez e cachear um delegate/expression compilada.",
                    "É exatamente assim que o EF Core materializa entidades e que o System.Text.Json/ASP.NET fazem binding: geram delegates em runtime na 1ª vez.",
                    "dynamic só quando realmente não há tipo em compilação (interop, JSON dinâmico). Perde checagem do compilador e IntelliSense.",
                    "Source generators (veja /estudos/runtime/serializacao) movem esse trabalho para a COMPILAÇÃO: zero reflection em runtime, compatível com AOT/trimming."
                }));
        }

        // ================================================================================================
        // 2) EXPRESSION TREES: FILTRO DINÂMICO TRADUZIDO PARA SQL
        // ================================================================================================
        private static readonly string[] CamposPermitidos = { "Camisa", "ValorMercado", "Nome", "Posicao", "Status", "Nacionalidade" };

        public async Task<ExperimentoResultado> FiltroDinamicoAsync(string campo, string operador, string valor, CancellationToken ct)
        {
            // WHITELIST: nunca monte expressão a partir de texto livre sem validar (superfície de ataque / DoS).
            var nomeCampo = CamposPermitidos.FirstOrDefault(c => c.Equals(campo, StringComparison.OrdinalIgnoreCase))
                            ?? throw new ArgumentException($"Campo '{campo}' não permitido. Use: {string.Join(", ", CamposPermitidos)}.");

            var propriedade = typeof(JogadorQM).GetProperty(nomeCampo)!;

            // x => x.<Campo> <operador> <valor>      (construída em RUNTIME, nó a nó)
            var x = Expression.Parameter(typeof(JogadorQM), "x");
            var acesso = Expression.Property(x, propriedade);
            object convertido;
            try { convertido = Convert.ChangeType(valor, propriedade.PropertyType, System.Globalization.CultureInfo.InvariantCulture)!; }
            catch (Exception) { throw new ArgumentException($"Valor '{valor}' inválido para o campo {nomeCampo} ({propriedade.PropertyType.Name})."); }
            var constante = Expression.Constant(convertido, propriedade.PropertyType);

            Expression corpo = operador.ToLowerInvariant() switch
            {
                "eq" => Expression.Equal(acesso, constante),
                "gt" => Expression.GreaterThan(acesso, constante),
                "lt" => Expression.LessThan(acesso, constante),
                "contains" when propriedade.PropertyType == typeof(string)
                    => Expression.Call(acesso, typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!, constante),
                _ => throw new ArgumentException("Operador inválido. Use: eq, gt, lt (ou contains para texto).")
            };

            var predicado = Expression.Lambda<Func<JogadorQM, bool>>(corpo, x);

            await using var db = await _factory.CreateDbContextAsync(ct);
            var consulta = db.Jogadores.Where(predicado);       // Expression<Func<>> => o EF traduz para SQL
            var sql = consulta.ToQueryString();
            var resultados = await consulta.OrderBy(j => j.Camisa).Take(10)
                .Select(j => new { j.Nome, j.Camisa, j.Posicao, j.ValorMercado }).ToListAsync(ct);

            return new ExperimentoResultado("Filtro dinâmico com Expression Trees",
                "Expression<Func<T,bool>> x Func<T,bool>; árvore de expressão -> SQL",
                new[]
                {
                    new Medicao("Resultado", $"x => x.{nomeCampo} {operador} {valor}", 0, 0, 1, new Dictionary<string, object?>
                    {
                        ["expressaoMontadaEmRuntime"] = predicado.ToString(),
                        ["sqlGerado"] = sql.ReplaceLineEndings(" "),
                        ["primeirosResultados"] = resultados
                    })
                },
                new[]
                {
                    "Func<T,bool> é CÓDIGO compilado (caixa-preta): só dá para executar em memória.",
                    "Expression<Func<T,bool>> é DADO (uma árvore): o EF percorre os nós e gera SQL. É por isso que 'IQueryable.Where' aceita lambda mas traduz para SQL.",
                    "Aqui montamos a árvore na mão a partir de texto: base de telas de 'filtro avançado' e de bibliotecas como OData/Dynamic LINQ.",
                    "Segurança: o campo passa por WHITELIST e o valor vira PARÂMETRO SQL (não concatenação) - sem SQL injection."
                });
        }

        // ================================================================================================
        // 3) SERIALIZAÇÃO: REFLECTION x SOURCE GENERATOR
        // ================================================================================================
        public Task<ExperimentoResultado> SerializacaoAsync(int itens, CancellationToken ct)
        {
            itens = Math.Clamp(itens, 100, 50_000);
            var dados = Enumerable.Range(0, itens)
                .Select(i => new JogadorResumoDto(Guid.NewGuid(), $"Jogador {i}", i % 3 == 0 ? "Apelido" : null, i % 99 + 1, "Atacante", "Ativo", i * 1000m))
                .ToList();

            var opcoesReflection = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            var opcoesGerado = new JsonSerializerOptions(JsonSerializerDefaults.Web) { TypeInfoResolver = LabJsonContext.Default };

            var medicoes = new List<Medicao>
            {
                Medidor.Medir("1. System.Text.Json por REFLECTION", "Descobre as propriedades em runtime (cacheia após a 1ª vez)", () =>
                {
                    var json = JsonSerializer.Serialize(dados, opcoesReflection);
                    return new Dictionary<string, object?> { ["bytes"] = json.Length };
                }),

                Medidor.Medir("2. System.Text.Json com SOURCE GENERATOR", "Código de serialização gerado na COMPILAÇÃO (ver LabJsonContext)", () =>
                {
                    var json = JsonSerializer.Serialize(dados, LabJsonContext.Default.ListJogadorResumoDto);
                    return new Dictionary<string, object?> { ["bytes"] = json.Length };
                })
            };

            return Task.FromResult(new ExperimentoResultado("Serialização: reflection x source generator",
                "Reflection em runtime x geração de código em compilação (AOT-friendly)",
                medicoes,
                new[]
                {
                    "O source generator elimina o custo de reflection na inicialização e reduz alocações; é requisito para Native AOT e trimming.",
                    "Em serialização em regime (após aquecer), a diferença de throughput é modesta; o ganho grande é o START-UP e a memória.",
                    "Mesmo conceito aparece em: Regex gerado (GeneratedRegex), logging gerado (LoggerMessage), Mapperly (mapeamento) etc."
                }));
        }

        // ================================================================================================
        // 4) INFORMAÇÕES DO RUNTIME
        // ================================================================================================
        public ExperimentoResultado InfoRuntime()
        {
            var gc = GC.GetGCMemoryInfo();
            var info = new Dictionary<string, object?>
            {
                ["runtime"] = RuntimeInformation.FrameworkDescription,
                ["so"] = RuntimeInformation.OSDescription,
                ["arquitetura"] = RuntimeInformation.ProcessArchitecture.ToString(),
                ["processadores"] = Environment.ProcessorCount,
                ["gcServidor"] = GCSettings.IsServerGC,
                ["gcLatencyMode"] = GCSettings.LatencyMode.ToString(),
                ["gcColetas(gen0/gen1/gen2)"] = $"{GC.CollectionCount(0)}/{GC.CollectionCount(1)}/{GC.CollectionCount(2)}",
                ["heapAtualMB"] = Math.Round(GC.GetTotalMemory(false) / 1024.0 / 1024.0, 1),
                ["heapTotalAlocadoDesdeInicioMB"] = Math.Round(GC.GetTotalAllocatedBytes(false) / 1024.0 / 1024.0, 1),
                ["memoriaDoProcessoMB"] = Math.Round(Environment.WorkingSet / 1024.0 / 1024.0, 1),
                ["pausaTotalGCms"] = Math.Round(GC.GetTotalPauseDuration().TotalMilliseconds, 1),
                ["fragmentacaoHeapMB"] = Math.Round(gc.FragmentedBytes / 1024.0 / 1024.0, 1),
                ["tieredCompilation"] = AppContext.TryGetSwitch("System.Runtime.TieredCompilation", out var tc) ? tc : "padrão (ligado)",
                ["threadsNoPool"] = ThreadPool.ThreadCount,
                ["assembliesCarregados"] = AppDomain.CurrentDomain.GetAssemblies().Length
            };

            return new ExperimentoResultado("Informações do runtime (CLR)", "JIT, GC (workstation x server), memória do processo",
                new[] { new Medicao("Runtime agora", "Instantâneo do processo da API", 0, 0, 0, info) },
                new[]
                {
                    "JIT em camadas (tiered): o código começa compilado rápido e pouco otimizado (tier-0) e é recompilado otimizado se ficar 'quente' (tier-1/PGO). Por isso a 1ª chamada é sempre mais lenta - e por isso há aquecimento nos experimentos.",
                    "GC Workstation x Server: Server usa 1 heap/thread de GC por núcleo (maior throughput, mais memória). APIs em contêiner costumam usar Server em instâncias maiores.",
                    "Memória do processo (working set) > heap gerenciado: inclui código JIT, bibliotecas nativas, pilhas das threads."
                });
        }
    }
}
