
using Galo.Domain.Torcida.Enums;
using Galo.Estudos.Common;
using Galo.Estudos.Persistence;
using Galo.Infrastructure.Persistence;
using Galo.Infrastructure.Persistence.QueryModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Galo.Estudos
{
    /// <summary>
    /// LABORATÓRIO DE ENTITY FRAMEWORK CORE.
    /// Cada método devolve várias VARIAÇÕES da mesma tarefa, medidas lado a lado: tracking, lazy/eager/explicit loading,
    /// projeção, consultas compiladas, operações em massa e paginação.
    /// </summary>
    public sealed class EfCoreLab
    {
        private readonly IServiceScopeFactory _scopes;
        private readonly IDbContextFactory<GaloQueryDbContext> _queryFactory;

        public EfCoreLab(IServiceScopeFactory scopes, IDbContextFactory<GaloQueryDbContext> queryFactory)
        {
            _scopes = scopes;
            _queryFactory = queryFactory;
        }

        // ================================================================================================
        // 1) TRACKING x NO-TRACKING
        // ================================================================================================
        /// <summary>
        /// O ChangeTracker guarda um SNAPSHOT de cada entidade lida para depois detectar o que mudou (dirty checking).
        /// Isso custa memória e CPU. Em consultas SOMENTE-LEITURA, AsNoTracking elimina esse custo.
        /// </summary>
        public async Task<ExperimentoResultado> TrackingAsync(int quantidade, CancellationToken ct)
        {
            quantidade = Math.Clamp(quantidade, 100, 20_000);

            async Task<IDictionary<string, object?>?> Executar(bool tracking, bool incluir, CancellationToken token)
            {
                // contexto NOVO por execução: o ChangeTracker começa vazio e não "contamina" a medição seguinte
                await using var escopo = _scopes.CreateAsyncScope();
                var db = escopo.ServiceProvider.GetRequiredService<EstudosDbContext>();

                IQueryable<TorcedorQM> q = db.Set<TorcedorQM>().OrderBy(t => t.Id).Take(quantidade);
                if (incluir) q = q.Include(t => t.Enderecos);
                q = tracking ? q.AsTracking() : q.AsNoTracking();

                var lista = await q.ToListAsync(token);
                return new Dictionary<string, object?>
                {
                    ["linhasLidas"] = lista.Count,
                    ["entidadesRastreadas"] = db.ChangeTracker.Entries().Count()
                };
            }

            var medicoes = new List<Medicao>
            {
                await Medidor.MedirAsync("1. Com tracking (padrão do EF)", "ToListAsync() normal: cada linha vira entidade rastreada",
                    t => Executar(true, false, t), ct),
                await Medidor.MedirAsync("2. AsNoTracking", "Mesma consulta sem snapshot/identity map",
                    t => Executar(false, false, t), ct),
                await Medidor.MedirAsync("3. Com tracking + Include(Enderecos)", "Rastreia torcedores E endereços",
                    t => Executar(true, true, t), ct),
                await Medidor.MedirAsync("4. AsNoTracking + Include(Enderecos)", "Mesmo Include, sem rastreamento",
                    t => Executar(false, true, t), ct)
            };

            return new ExperimentoResultado("Tracking x NoTracking",
                "ChangeTracker, snapshot, identity map, dirty checking",
                medicoes,
                new[]
                {
                    "Compare 'alocadoKb' e 'entidadesRastreadas': sem tracking o EF não guarda snapshot nem registra nada no ChangeTracker.",
                    "Regra prática: LEITURA = AsNoTracking (ou um DbContext de leitura como o GaloQueryDbContext). ESCRITA = com tracking.",
                    "AsNoTrackingWithIdentityResolution existe para o meio-termo: sem rastrear, mas sem duplicar instâncias do mesmo registro.",
                    "O custo cresce com o número de entidades e com o tamanho do grafo (Includes)."
                });
        }

        // ================================================================================================
        // 2) LAZY x EAGER x EXPLICIT x PROJEÇÃO  (o problema N+1)
        // ================================================================================================
        public async Task<ExperimentoResultado> CarregamentoAsync(int partidas, CancellationToken ct)
        {
            partidas = Math.Clamp(partidas, 5, 200);

            // Tarefa: para N partidas, ler o nome do mandante, do visitante e quantos gols teve.
            var medicoes = new List<Medicao>
            {
                // --- LAZY: o loop dispara SELECTs escondidos ---
                await Medidor.MedirAsync("1. LAZY LOADING (proxies)",
                    "Carrega só as partidas; ao tocar em p.Mandante/p.Visitante/p.Gols o EF faz um SELECT por acesso => N+1",
                    async t =>
                    {
                        await using var escopo = _scopes.CreateAsyncScope();
                        var db = escopo.ServiceProvider.GetRequiredService<EstudosDbContext>();
                        var lista = await db.Set<PartidaQM>().OrderBy(p => p.DataHoraUtc).Take(partidas).ToListAsync(t);

                        var gols = 0;
                        foreach (var p in lista)
                        {
                            _ = p.Mandante.Nome;      // <- SELECT escondido
                            _ = p.Visitante.Nome;     // <- SELECT escondido
                            gols += p.Gols.Count;     // <- SELECT escondido
                        }
                        return new Dictionary<string, object?> { ["partidas"] = lista.Count, ["totalGols"] = gols };
                    }, ct),

                // --- EXPLICIT: você decide quando carregar (continua 1 consulta por item) ---
                await Medidor.MedirAsync("2. EXPLICIT LOADING",
                    "Entry(p).Reference().LoadAsync(): carregamento sob demanda, mas ainda 1 consulta por item",
                    async t =>
                    {
                        await using var escopo = _scopes.CreateAsyncScope();
                        var db = escopo.ServiceProvider.GetRequiredService<EstudosDbContext>();
                        var lista = await db.Set<PartidaQM>().OrderBy(p => p.DataHoraUtc).Take(partidas).ToListAsync(t);

                        var gols = 0;
                        foreach (var p in lista)
                        {
                            await db.Entry(p).Reference(x => x.Mandante).LoadAsync(t);
                            await db.Entry(p).Reference(x => x.Visitante).LoadAsync(t);
                            await db.Entry(p).Collection(x => x.Gols).LoadAsync(t);
                            gols += p.Gols.Count;
                        }
                        return new Dictionary<string, object?> { ["partidas"] = lista.Count, ["totalGols"] = gols };
                    }, ct),

                // --- EAGER: Include => 1 consulta com JOINs ---
                await Medidor.MedirAsync("3. EAGER LOADING (Include)",
                    "Include(Mandante, Visitante, Gols): UMA consulta com JOIN. Atenção: a coleção Gols multiplica as linhas do resultado",
                    async t =>
                    {
                        await using var escopo = _scopes.CreateAsyncScope();
                        var db = escopo.ServiceProvider.GetRequiredService<EstudosDbContext>();
                        var lista = await db.Set<PartidaQM>().AsNoTracking()
                            .Include(p => p.Mandante).Include(p => p.Visitante).Include(p => p.Gols)
                            .OrderBy(p => p.DataHoraUtc).Take(partidas).ToListAsync(t);

                        return new Dictionary<string, object?> { ["partidas"] = lista.Count, ["totalGols"] = lista.Sum(p => p.Gols.Count) };
                    }, ct),

                // --- EAGER + SPLIT ---
                await Medidor.MedirAsync("4. EAGER + AsSplitQuery",
                    "Mesmo Include, mas separado em 2-3 SELECTs simples: evita o produto cartesiano de coleções",
                    async t =>
                    {
                        await using var escopo = _scopes.CreateAsyncScope();
                        var db = escopo.ServiceProvider.GetRequiredService<EstudosDbContext>();
                        var lista = await db.Set<PartidaQM>().AsNoTracking()
                            .Include(p => p.Mandante).Include(p => p.Visitante).Include(p => p.Gols)
                            .AsSplitQuery()
                            .OrderBy(p => p.DataHoraUtc).Take(partidas).ToListAsync(t);

                        return new Dictionary<string, object?> { ["partidas"] = lista.Count, ["totalGols"] = lista.Sum(p => p.Gols.Count) };
                    }, ct),

                // --- PROJEÇÃO: só as colunas que a tela usa ---
                await Medidor.MedirAsync("5. PROJEÇÃO (Select para DTO)",
                    "Select(new {...}): UMA consulta, só as colunas necessárias, sem entidades. É a opção mais eficiente para telas/APIs de leitura",
                    async t =>
                    {
                        await using var db = await _queryFactory.CreateDbContextAsync(t);
                        var lista = await db.Partidas
                            .OrderBy(p => p.DataHoraUtc).Take(partidas)
                            .Select(p => new { Mandante = p.Mandante.Nome, Visitante = p.Visitante.Nome, Gols = p.Gols.Count })
                            .ToListAsync(t);

                        return new Dictionary<string, object?> { ["partidas"] = lista.Count, ["totalGols"] = lista.Sum(p => p.Gols) };
                    }, ct)
            };

            return new ExperimentoResultado("Lazy x Eager x Explicit x Projeção (N+1)",
                "Estratégias de carregamento de dados relacionados e o problema N+1",
                medicoes,
                new[]
                {
                    "Olhe a coluna 'comandosSql': o lazy/explicit executam 1 + ~3N consultas; o eager 1 (ou 3 com split); a projeção 1.",
                    "N+1 = 1 consulta para a lista + N consultas para os relacionados. Com 200 partidas e latência de 2 ms por ida ao banco, são ~1 s só de rede.",
                    "Lazy loading não faz parte do EF por padrão (exige Microsoft.EntityFrameworkCore.Proxies + navegações virtual). É de propósito: ele esconde o custo.",
                    "AsSplitQuery troca 1 consulta gigante (linhas duplicadas por causa dos JOINs de coleção) por várias pequenas; vale quando há 2+ coleções.",
                    "Projeção ganha porque o banco devolve só 3 colunas por partida e o EF não cria/rastreia entidades."
                });
        }

        // ================================================================================================
        // 3) PROJEÇÃO, COUNT/ANY, CONSULTA COMPILADA
        // ================================================================================================
        private static readonly Func<GaloQueryDbContext, Guid, Task<string?>> ObterNomeCompilada =
            EF.CompileAsyncQuery((GaloQueryDbContext c, Guid id) =>
                c.Torcedores.Where(t => t.Id == id).Select(t => t.Nome).FirstOrDefault());

        public async Task<ExperimentoResultado> ProjecaoAsync(int quantidade, CancellationToken ct)
        {
            quantidade = Math.Clamp(quantidade, 100, 20_000);

            await using var preparo = await _queryFactory.CreateDbContextAsync(ct);
            var ids = await preparo.Torcedores.OrderBy(t => t.Id).Select(t => t.Id).Take(300).ToListAsync(ct);

            var medicoes = new List<Medicao>
            {
                await Medidor.MedirAsync("Entidade completa", "SELECT de todas as colunas do torcedor (AsNoTracking)",
                    async t =>
                    {
                        await using var db = await _queryFactory.CreateDbContextAsync(t);
                        var l = await db.Torcedores.OrderBy(x => x.Id).Take(quantidade).ToListAsync(t);
                        return new Dictionary<string, object?> { ["linhas"] = l.Count };
                    }, ct),

                await Medidor.MedirAsync("Projeção anônima (3 colunas)", "Select(x => new { x.Id, x.Nome, x.Plano })",
                    async t =>
                    {
                        await using var db = await _queryFactory.CreateDbContextAsync(t);
                        var l = await db.Torcedores.OrderBy(x => x.Id).Take(quantidade)
                            .Select(x => new { x.Id, x.Nome, x.Plano }).ToListAsync(t);
                        return new Dictionary<string, object?> { ["linhas"] = l.Count };
                    }, ct),

                await Medidor.MedirAsync("COUNT ruim: ToList().Count", "Traz TODAS as linhas só para contar",
                    async t =>
                    {
                        await using var db = await _queryFactory.CreateDbContextAsync(t);
                        var total = (await db.Torcedores.ToListAsync(t)).Count;
                        return new Dictionary<string, object?> { ["total"] = total };
                    }, ct),

                await Medidor.MedirAsync("COUNT bom: CountAsync()", "SELECT COUNT(*) no banco",
                    async t =>
                    {
                        await using var db = await _queryFactory.CreateDbContextAsync(t);
                        return new Dictionary<string, object?> { ["total"] = await db.Torcedores.CountAsync(t) };
                    }, ct),

                await Medidor.MedirAsync("EXISTS: AnyAsync()", "Para 'existe algum?' use Any: o banco para na 1ª linha",
                    async t =>
                    {
                        await using var db = await _queryFactory.CreateDbContextAsync(t);
                        return new Dictionary<string, object?> { ["existe"] = await db.Torcedores.AnyAsync(x => x.Plano == "Diamante", t) };
                    }, ct),

                await Medidor.MedirAsync("1000 consultas SEM compilação", "Cada chamada traduz a expressão LINQ -> SQL (há cache de plano, mas ainda há trabalho)",
                    async t =>
                    {
                        await using var db = await _queryFactory.CreateDbContextAsync(t);
                        for (var i = 0; i < 1000; i++)
                            _ = await db.Torcedores.Where(x => x.Id == ids[i % ids.Count]).Select(x => x.Nome).FirstOrDefaultAsync(t);
                        return new Dictionary<string, object?> { ["execucoes"] = 1000 };
                    }, ct),

                await Medidor.MedirAsync("1000 consultas COMPILADAS (EF.CompileAsyncQuery)", "A tradução LINQ -> SQL é feita UMA vez e reaproveitada",
                    async t =>
                    {
                        await using var db = await _queryFactory.CreateDbContextAsync(t);
                        for (var i = 0; i < 1000; i++) _ = await ObterNomeCompilada(db, ids[i % ids.Count]);
                        return new Dictionary<string, object?> { ["execucoes"] = 1000 };
                    }, ct)
            };

            return new ExperimentoResultado("Projeção, COUNT/EXISTS e consultas compiladas",
                "Buscar menos dados; deixar o banco agregar; evitar retraduzir LINQ",
                medicoes,
                new[]
                {
                    "Projeção reduz tráfego de rede e alocação: o resultado não carrega as colunas que ninguém vai usar.",
                    "ToList().Count x CountAsync é o erro clássico de 'filtrar/agregar no cliente'. O banco faz isso muito melhor.",
                    "Any() vira EXISTS e pode parar na primeira linha; Count() > 0 conta tudo.",
                    "Consultas compiladas ganham em ALTA FREQUÊNCIA de consultas pequenas. Em consultas pesadas, o tempo é do banco, e a diferença some."
                });
        }

        // ================================================================================================
        // 4) OPERAÇÕES EM MASSA (com ROLLBACK: não altera seus dados)
        // ================================================================================================
        public async Task<ExperimentoResultado> OperacoesEmMassaAsync(int quantidade, CancellationToken ct)
        {
            quantidade = Math.Clamp(quantidade, 50, 5_000);

            async Task<IDictionary<string, object?>?> TrackingUpdate(CancellationToken t)
            {
                await using var escopo = _scopes.CreateAsyncScope();
                var db = escopo.ServiceProvider.GetRequiredService<GaloDbContext>();

                // Transação própria + ROLLBACK ao final: mede sem alterar os dados.
                // (Com EnableRetryOnFailure, transação manual exige CreateExecutionStrategy().ExecuteAsync.)
                var estrategia = db.Database.CreateExecutionStrategy();
                return await estrategia.ExecuteAsync(async () =>
                {
                    await using var tx = await db.Database.BeginTransactionAsync(t);
                    var lote = await db.Torcedores.Where(x => x.Plano == PlanoSocio.Nenhum).OrderBy(x => x.Id).Take(quantidade).ToListAsync(t);

                    foreach (var torcedor in lote) torcedor.AlterarPlano(PlanoSocio.Bronze);   // passa pelas regras do domínio
                    var linhas = await db.SaveChangesAsync(t);

                    await tx.RollbackAsync(t);
                    db.ChangeTracker.Clear();
                    return (IDictionary<string, object?>?)new Dictionary<string, object?> { ["carregados"] = lote.Count, ["linhasAfetadas"] = linhas };
                });
            }

            async Task<IDictionary<string, object?>?> BulkUpdate(CancellationToken t)
            {
                await using var escopo = _scopes.CreateAsyncScope();
                var db = escopo.ServiceProvider.GetRequiredService<GaloDbContext>();

                var estrategia = db.Database.CreateExecutionStrategy();
                return await estrategia.ExecuteAsync(async () =>
                {
                    await using var tx = await db.Database.BeginTransactionAsync(t);

                    // UMA instrução UPDATE ... WHERE, direto no banco. Nada é carregado em memória.
                    var linhas = await db.Torcedores
                        .Where(x => db.Torcedores.Where(y => y.Plano == PlanoSocio.Nenhum).OrderBy(y => y.Id).Take(quantidade).Select(y => y.Id).Contains(x.Id))
                        .ExecuteUpdateAsync(s => s.SetProperty(x => x.Plano, PlanoSocio.Bronze), t);

                    await tx.RollbackAsync(t);
                    return (IDictionary<string, object?>?)new Dictionary<string, object?> { ["linhasAfetadas"] = linhas };
                });
            }

            var medicoes = new List<Medicao>
            {
                await Medidor.MedirAsync("1. Carregar + alterar + SaveChanges",
                    "Passa pelo domínio (AlterarPlano valida regras e gera eventos). Faz SELECT + N UPDATEs agrupados em lotes",
                    TrackingUpdate, ct, aquecer: false),

                await Medidor.MedirAsync("2. ExecuteUpdateAsync (bulk)",
                    "1 comando UPDATE. MUITO mais rápido, mas IGNORA o domínio: sem validação, sem eventos, sem DataAtualizacao",
                    BulkUpdate, ct, aquecer: false)
            };

            return new ExperimentoResultado("Operações em massa (ExecuteUpdate/ExecuteDelete)",
                "Bulk operations x ChangeTracker; custo de passar pelo domínio",
                medicoes,
                new[]
                {
                    "As duas operações rodam dentro de uma transação que sofre ROLLBACK: seus dados não são alterados.",
                    "ExecuteUpdate/ExecuteDelete (EF Core 7+) não carregam nem rastreiam entidades: ótimos para manutenção em lote.",
                    "O preço: pular o domínio. Regras (Guard), eventos de domínio e DataAtualizacao NÃO acontecem. Use com consciência (jobs de manutenção, não casos de uso).",
                    "SaveChanges agrupa os UPDATEs em lotes (batching) - ainda assim há 1 UPDATE lógico por linha."
                });
        }

        // ================================================================================================
        // 5) PAGINAÇÃO: OFFSET x KEYSET
        // ================================================================================================
        public async Task<ExperimentoResultado> PaginacaoAsync(int tamanhoPagina, CancellationToken ct)
        {
            tamanhoPagina = Math.Clamp(tamanhoPagina, 5, 100);

            await using var preparo = await _queryFactory.CreateDbContextAsync(ct);
            var total = await preparo.Partidas.CountAsync(ct);
            var pular = Math.Max(0, total - tamanhoPagina);   // "última página"

            // cursor da keyset = o item imediatamente anterior à última página
            var cursor = pular == 0
                ? null
                : await preparo.Partidas.OrderByDescending(p => p.DataHoraUtc).ThenBy(p => p.Id)
                    .Skip(pular - 1).Select(p => new { p.DataHoraUtc, p.Id }).FirstAsync(ct);

            var medicoes = new List<Medicao>
            {
                await Medidor.MedirAsync("OFFSET/LIMIT (Skip/Take)", $"Pula {pular} linhas (o banco precisa LER e descartar todas)",
                    async t =>
                    {
                        await using var db = await _queryFactory.CreateDbContextAsync(t);
                        var l = await db.Partidas.OrderByDescending(p => p.DataHoraUtc).ThenBy(p => p.Id)
                            .Skip(pular).Take(tamanhoPagina).Select(p => p.Id).ToListAsync(t);
                        return new Dictionary<string, object?> { ["linhas"] = l.Count, ["totalTabela"] = total };
                    }, ct),

                await Medidor.MedirAsync("KEYSET (seek)", "WHERE (data, id) < cursor: usa o índice para ir direto ao ponto, sem ler o que veio antes",
                    async t =>
                    {
                        await using var db = await _queryFactory.CreateDbContextAsync(t);
                        var q = db.Partidas.AsQueryable();
                        if (cursor is not null)
                            q = q.Where(p => p.DataHoraUtc < cursor.DataHoraUtc
                                          || (p.DataHoraUtc == cursor.DataHoraUtc && p.Id.CompareTo(cursor.Id) > 0));

                        var l = await q.OrderByDescending(p => p.DataHoraUtc).ThenBy(p => p.Id)
                            .Take(tamanhoPagina).Select(p => p.Id).ToListAsync(t);
                        return new Dictionary<string, object?> { ["linhas"] = l.Count, ["totalTabela"] = total };
                    }, ct)
            };

            return new ExperimentoResultado("Paginação: Offset x Keyset",
                "Custo de Skip em páginas profundas; paginação por cursor",
                medicoes,
                new[]
                {
                    "Com poucos milhares de linhas a diferença é pequena. Com milhões, OFFSET degrada linearmente com a profundidade da página.",
                    "KEYSET exige ordenação ESTÁVEL e única (data + id) e um índice compatível; não permite 'pular para a página 37'.",
                    "A API deste projeto usa OFFSET (simples e suficiente para o volume); para feeds/infinite scroll, prefira keyset."
                });
        }
    }
}
