using System.Buffers;
using System.Text;
using Galo.Estudos.Common;
using Galo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Galo.Estudos
{
    /// <summary>
    /// LABORATÓRIO DE MEMÓRIA / ALOCAÇÃO.
    /// Conceitos: onde o filtro acontece (banco x memória), estruturas de dados, alocações no heap, Garbage Collector,
    /// Span&lt;T&gt;, ArrayPool, LINQ x laço.
    /// </summary>
    public sealed class MemoriaLab
    {
        private readonly IDbContextFactory<GaloQueryDbContext> _factory;
        public MemoriaLab(IDbContextFactory<GaloQueryDbContext> factory) => _factory = factory;

        // ================================================================================================
        // 1) FILTRAR NO BANCO x FILTRAR EM MEMÓRIA
        // ================================================================================================
        public async Task<ExperimentoResultado> ServidorVsClienteAsync(CancellationToken ct)
        {
            await using var preparo = await _factory.CreateDbContextAsync(ct);
            var galoId = await preparo.Clubes.Where(c => c.EhOGalo).Select(c => c.Id).FirstOrDefaultAsync(ct);
            if (galoId == Guid.Empty) throw new InvalidOperationException("Sem dados. Execute POST /api/v1/estudos/seed antes.");

            var medicoes = new List<Medicao>
            {
                await Medidor.MedirAsync("1. Filtro NO BANCO (Where antes do ToList)", "O PostgreSQL filtra e devolve só as partidas do Galo",
                    async t =>
                    {
                        await using var db = await _factory.CreateDbContextAsync(t);
                        var l = await db.Partidas.Where(p => p.MandanteId == galoId || p.VisitanteId == galoId).ToListAsync(t);
                        return new Dictionary<string, object?> { ["linhasTrafegadas"] = l.Count };
                    }, ct),

                await Medidor.MedirAsync("2. Filtro EM MEMÓRIA (ToList e depois Where)", "Traz a tabela INTEIRA para a API e filtra em LINQ to Objects",
                    async t =>
                    {
                        await using var db = await _factory.CreateDbContextAsync(t);
                        var todas = await db.Partidas.ToListAsync(t);                                   // <- tudo trafega
                        var doGalo = todas.Where(p => p.MandanteId == galoId || p.VisitanteId == galoId).ToList();
                        return new Dictionary<string, object?> { ["linhasTrafegadas"] = todas.Count, ["linhasUteis"] = doGalo.Count };
                    }, ct),

                await Medidor.MedirAsync("3. AsEnumerable() no meio da consulta", "A armadilha: o Where depois do AsEnumerable já roda no cliente (LINQ to Objects)",
                    async t =>
                    {
                        await using var db = await _factory.CreateDbContextAsync(t);
                        var l = db.Partidas.AsEnumerable().Where(p => p.MandanteId == galoId || p.VisitanteId == galoId).ToList();
                        return new Dictionary<string, object?> { ["linhasUteis"] = l.Count, ["observacao"] = "leitura síncrona da tabela inteira" };
                    }, ct)
            };

            return new ExperimentoResultado("Filtrar no banco x filtrar em memória",
                "IQueryable x IEnumerable, client evaluation, tráfego de rede e memória",
                medicoes,
                new[]
                {
                    "IQueryable<T> = a consulta ainda é uma EXPRESSÃO que o EF traduz para SQL. IEnumerable<T> = os dados já estão na memória.",
                    "Qualquer ToList()/AsEnumerable() 'fecha' a consulta: o que vier depois (Where, OrderBy...) roda em memória.",
                    "Com 400 linhas a diferença é pequena; com 10 milhões, a variação 2 derruba a API por falta de memória.",
                    "Regra: filtre, ordene, pagine e agregue NO BANCO; só materialize o resultado final."
                });
        }

        // ================================================================================================
        // 2) ESTRUTURAS DE DADOS: List x HashSet x Dictionary
        // ================================================================================================
        public Task<ExperimentoResultado> ColecoesAsync(int tamanho, CancellationToken ct)
        {
            tamanho = Math.Clamp(tamanho, 1_000, 200_000);
            var lista = Enumerable.Range(0, tamanho).Select(i => Guid.NewGuid()).ToList();
            var conjunto = new HashSet<Guid>(lista);
            var dicionario = lista.ToDictionary(g => g, g => 1);
            var buscas = lista.Where((_, i) => i % 50 == 0).Concat(Enumerable.Range(0, 100).Select(_ => Guid.NewGuid())).ToList();

            var medicoes = new List<Medicao>
            {
                Medidor.Medir("List<T>.Contains", $"Busca LINEAR O(n): {buscas.Count} buscas em {tamanho} itens", () =>
                {
                    var achou = 0;
                    foreach (var b in buscas) if (lista.Contains(b)) achou++;
                    return new Dictionary<string, object?> { ["achados"] = achou };
                }),

                Medidor.Medir("HashSet<T>.Contains", "Busca por HASH O(1) médio", () =>
                {
                    var achou = 0;
                    foreach (var b in buscas) if (conjunto.Contains(b)) achou++;
                    return new Dictionary<string, object?> { ["achados"] = achou };
                }),

                Medidor.Medir("Dictionary<K,V>.TryGetValue", "Também O(1); além de existir, devolve o valor associado", () =>
                {
                    var achou = 0;
                    foreach (var b in buscas) if (dicionario.TryGetValue(b, out _)) achou++;
                    return new Dictionary<string, object?> { ["achados"] = achou };
                })
            };

            return Task.FromResult(new ExperimentoResultado("List x HashSet x Dictionary",
                "Complexidade algorítmica (O(n) x O(1)) na prática",
                medicoes,
                new[]
                {
                    "Um 'ids.Contains(x)' dentro de um laço sobre uma List é o clássico O(n²) escondido. Converta para HashSet antes do laço.",
                    "HashSet/Dictionary gastam mais memória (tabela de hash) para ganhar velocidade de busca: é uma troca espaço x tempo.",
                    "Para listas pequenas (<20 itens) a List costuma ser tão rápida quanto - a constante do hash pesa."
                }));
        }

        // ================================================================================================
        // 3) STRINGS E ALOCAÇÃO
        // ================================================================================================
        public Task<ExperimentoResultado> StringsAsync(int iteracoes, CancellationToken ct)
        {
            iteracoes = Math.Clamp(iteracoes, 1_000, 50_000);

            var medicoes = new List<Medicao>
            {
                Medidor.Medir("1. string += em laço", "string é IMUTÁVEL: cada += cria uma string nova e copia a anterior (O(n²) em memória)", () =>
                {
                    var s = string.Empty;
                    for (var i = 0; i < iteracoes; i++) s += "Galo,";
                    return new Dictionary<string, object?> { ["tamanhoFinal"] = s.Length };
                }),

                Medidor.Medir("2. StringBuilder", "Buffer mutável que cresce por dobra: poucas alocações", () =>
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i < iteracoes; i++) sb.Append("Galo,");
                    return new Dictionary<string, object?> { ["tamanhoFinal"] = sb.Length };
                }),

                Medidor.Medir("3. string.Concat de uma sequência", "Calcula o tamanho final e aloca UMA vez", () =>
                {
                    var s = string.Concat(Enumerable.Repeat("Galo,", iteracoes));
                    return new Dictionary<string, object?> { ["tamanhoFinal"] = s.Length };
                }),

                Medidor.Medir("4. Interpolação em laço (boxing + string por item)", "$\"{i}\" aloca uma string por iteração", () =>
                {
                    var total = 0;
                    for (var i = 0; i < iteracoes * 10; i++) total += $"item-{i}".Length;
                    return new Dictionary<string, object?> { ["caracteres"] = total };
                })
            };

            return Task.FromResult(new ExperimentoResultado("Strings e alocações",
                "Imutabilidade, heap, Garbage Collector (gerações 0/1/2), StringBuilder",
                medicoes,
                new[]
                {
                    "Compare 'alocadoKb' e as coletas de GC: concatenar em laço pode alocar centenas de MB para produzir poucos KB de resultado.",
                    "Objetos que vivem pouco morrem na geração 0 (barato). Objetos grandes (>85 KB) vão para o LOH (Large Object Heap), que só é limpo na gen2 (caro).",
                    "Alocação não é 'de graça': quanto mais lixo, mais o GC pausa a aplicação. Em APIs de alto volume, reduzir alocação reduz latência de cauda (p99)."
                }));
        }

        // ================================================================================================
        // 4) SPAN, STACKALLOC, ARRAYPOOL
        // ================================================================================================
        public Task<ExperimentoResultado> SpanEPoolAsync(int iteracoes, CancellationToken ct)
        {
            iteracoes = Math.Clamp(iteracoes, 10_000, 5_000_000);
            const string cpf = "123.456.789-09";

            var medicoes = new List<Medicao>
            {
                Medidor.Medir("1. Substring", "Cada Substring ALOCA uma string nova no heap", () =>
                {
                    long soma = 0;
                    for (var i = 0; i < iteracoes; i++)
                    {
                        var a = cpf.Substring(0, 3); var b = cpf.Substring(4, 3); var c = cpf.Substring(8, 3);
                        soma += a.Length + b.Length + c.Length;
                    }
                    return new Dictionary<string, object?> { ["soma"] = soma };
                }),

                Medidor.Medir("2. AsSpan().Slice", "Span é uma 'janela' sobre a memória existente: ZERO alocação", () =>
                {
                    long soma = 0;
                    for (var i = 0; i < iteracoes; i++)
                    {
                        var s = cpf.AsSpan();
                        var a = s.Slice(0, 3); var b = s.Slice(4, 3); var c = s.Slice(8, 3);
                        soma += a.Length + b.Length + c.Length;
                    }
                    return new Dictionary<string, object?> { ["soma"] = soma };
                }),

                Medidor.Medir("3. new byte[1024] a cada iteração", "Aloca um array novo no heap por iteração (pressão no GC)", () =>
                {
                    long soma = 0;
                    for (var i = 0; i < iteracoes / 10; i++) { var buf = new byte[1024]; buf[0] = 1; soma += buf.Length; }
                    return new Dictionary<string, object?> { ["bytesProcessados"] = soma };
                }),

                Medidor.Medir("4. ArrayPool<byte>.Shared", "Aluga e devolve buffers: reaproveita a memória", () =>
                {
                    long soma = 0;
                    for (var i = 0; i < iteracoes / 10; i++)
                    {
                        var buf = ArrayPool<byte>.Shared.Rent(1024);
                        try { buf[0] = 1; soma += 1024; }
                        finally { ArrayPool<byte>.Shared.Return(buf); }
                    }
                    return new Dictionary<string, object?> { ["bytesProcessados"] = soma };
                }),

                Medidor.Medir("5. stackalloc + Span", "Buffer pequeno NA PILHA: sem GC, some quando o método retorna", () =>
                {
                    long soma = 0;
                    for (var i = 0; i < iteracoes / 10; i++) soma += UsarBufferNaPilha();
                    return new Dictionary<string, object?> { ["bytesProcessados"] = soma };
                })
            };

            return Task.FromResult(new ExperimentoResultado("Span<T>, stackalloc e ArrayPool",
                "Alocação zero, ref structs, reaproveitamento de buffers",
                medicoes,
                new[]
                {
                    "Span<T> é uma ref struct: só vive na pilha, não pode ir para campos de classe nem atravessar um await.",
                    "stackalloc só para buffers PEQUENOS (a pilha tem ~1 MB por thread). ArrayPool para buffers maiores/variáveis.",
                    "Essas técnicas são para CAMINHOS QUENTES comprovados por medição (parsers, serialização, protocolos). No dia a dia, clareza primeiro."
                }));
        }

        /// <summary>
        /// ARMADILHA CLÁSSICA: memória de stackalloc só é liberada quando o MÉTODO retorna, não ao fim do bloco/iteração.
        /// Um stackalloc DENTRO de um laço acumularia 1 KB por volta até estourar a pilha (StackOverflow derruba o processo
        /// inteiro e não dá para capturar com try/catch). Por isso o buffer vive neste método pequeno, chamado a cada iteração.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static int UsarBufferNaPilha()
        {
            Span<byte> buf = stackalloc byte[1024];
            buf[0] = 1;
            return buf.Length;
        }

        // ================================================================================================
        // 5) LINQ x LAÇO
        // ================================================================================================
        public Task<ExperimentoResultado> LinqVsLacoAsync(int tamanho, CancellationToken ct)
        {
            tamanho = Math.Clamp(tamanho, 100_000, 20_000_000);
            var numeros = Enumerable.Range(1, tamanho).ToArray();

            var medicoes = new List<Medicao>
            {
                Medidor.Medir("1. LINQ encadeado", "Where + Select + Sum: iteradores, delegates e closures (n % 1000 evita overflow do Sum)", () =>
                    new Dictionary<string, object?> { ["soma"] = numeros.Where(n => n % 2 == 0).Select(n => (long)(n % 1000) * (n % 1000)).Sum() }),

                Medidor.Medir("2. foreach", "Laço simples sobre o array", () =>
                {
                    long soma = 0;
                    foreach (var n in numeros) if (n % 2 == 0) soma += (long)(n % 1000) * (n % 1000);
                    return new Dictionary<string, object?> { ["soma"] = soma };
                }),

                Medidor.Medir("3. for sobre Span", "Elimina a checagem de limites quando o JIT consegue provar o acesso", () =>
                {
                    long soma = 0;
                    var span = numeros.AsSpan();
                    for (var i = 0; i < span.Length; i++) { var n = span[i]; if (n % 2 == 0) soma += (long)(n % 1000) * (n % 1000); }
                    return new Dictionary<string, object?> { ["soma"] = soma };
                }),

                Medidor.Medir("4. LINQ com ToList() intermediário", "ToList() no meio materializa uma lista inteira à toa", () =>
                    new Dictionary<string, object?> { ["soma"] = numeros.Where(n => n % 2 == 0).ToList().Select(n => (long)(n % 1000) * (n % 1000)).ToList().Sum() })
            };

            return Task.FromResult(new ExperimentoResultado("LINQ x laço",
                "Execução adiada (deferred execution), alocação de iteradores, materialização desnecessária",
                medicoes,
                new[]
                {
                    "LINQ é legível e, na maioria dos casos, suficientemente rápido. A diferença só importa em caminhos quentes.",
                    "O erro real não é LINQ x for: é MATERIALIZAR cedo demais (variação 4) ou enumerar a mesma sequência várias vezes.",
                    "Execução adiada: um IEnumerable de LINQ só executa quando é enumerado - e executa DE NOVO a cada enumeração."
                }));
        }
    }
}
