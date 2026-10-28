# Galo.NET — laboratório de estudos de .NET 10 (Clube Atlético Mineiro)

Projeto de estudo: uma Web API em **.NET 10** com **DDD + CQRS**, **Entity Framework Core + PostgreSQL**, **Redis**,
**RabbitMQ**, **Keycloak (BFF)** e **OpenTelemetry**, sobre o domínio do **Clube Atlético Mineiro** (jogadores, partidas,
campeonatos, torcedores e venda de ingressos).

Além da API, existe um **laboratório** (`/api/v1/estudos/*`) que executa experimentos lado a lado e mostra tempo,
memória alocada e quantidade de comandos SQL — para *ver* conceitos como lazy loading, N+1, projeção, paralelismo,
cache, race condition e concorrência otimista, em vez de só ler sobre eles.

> Os dados são de **exemplo** (nomes e números fictícios gerados pelo seed). Clubes e estádios reais só dão contexto.

---

## 1. Como rodar

Pré-requisitos: .NET SDK 10, e os serviços de desenvolvimento já rodando em `localhost`
(PostgreSQL, Redis, RabbitMQ, Keycloak). **Nenhum segredo fica no repositório**: as credenciais vêm dos
*user-secrets* do projeto `Galo.Presentation` (`UserSecretsId` no `.csproj`).

```bash
dotnet run --project src/Galo.Presentation --launch-profile http     # http://localhost:5205 (abre o Swagger)
```

Em `Development` a API, ao subir, **aplica as migrations** (conexão `PostgresMigrations`, usuário *owner*).
Depois gere os dados de exemplo:

```bash
curl -X POST "http://localhost:5205/api/v1/estudos/seed"             # ~2 s: 20 clubes, 60 jogadores, 2000 torcedores, 400 partidas...
curl -X POST "http://localhost:5205/api/v1/estudos/seed?torcedores=50000&partidasEncerradas=20000"   # mais volume
curl -X DELETE "http://localhost:5205/api/v1/estudos/seed"           # apaga tudo (recomeçar)
```

| URL | Para quê |
|---|---|
| `/swagger` | Documentação interativa (somente Development) |
| `/health` | Saúde de postgres, redis, rabbitmq e keycloak (anônimo, sem detalhes sensíveis) |
| `/metrics` | Métricas no formato Prometheus (anônimo) |

Testes de domínio: `dotnet test` (69 testes, sem dependências externas).

### Chaves de configuração lidas (todas nos user-secrets em Development)

`ConnectionStrings:Postgres` (runtime, usuário sem DDL) · `ConnectionStrings:PostgresMigrations` (só migrations) ·
`ConnectionStrings:Redis` · `Redis:KeyPrefix` · `RabbitMQ:Host/Port/VirtualHost/UserName/Password` ·
`Keycloak:Authority/Realm/ClientId/ClientSecret` · `OTEL_EXPORTER_OTLP_ENDPOINT/PROTOCOL` · `OTEL_RESOURCE_ATTRIBUTES`.

Configurações **não secretas** ficam em `appsettings*.json`: `Auth:Enforce` (liga/desliga a exigência de login — `false` em
Development para estudar pelo Swagger), `Database:MigrateOnStartup`, `Frontend:Origin`, nomes de exchange/fila.

### Migrations

```bash
dotnet ef migrations add NomeDaMigration -p src/Galo.Infrastructure -s src/Galo.Presentation -c GaloDbContext -o Persistence/Migrations
dotnet ef migrations script -p src/Galo.Infrastructure -s src/Galo.Presentation -c GaloDbContext      # só gera o SQL
```

O `GaloDbContextFactory` (design-time) lê `ConnectionStrings:PostgresMigrations` dos user-secrets.
**Princípio do menor privilégio:** a API em runtime usa o usuário `app` (só SELECT/INSERT/UPDATE/DELETE); só as migrations
usam o usuário *owner* (DDL).

---

## 2. Arquitetura

```
┌────────────────────────────── Galo.Presentation ──────────────────────────────┐
│ Controllers finos · Auth BFF (Keycloak) · OpenTelemetry · /health · /metrics  │
└───────┬───────────────────────────────┬───────────────────────────────┬───────┘
        │ MediatR                       │                               │
┌───────▼─────────┐            ┌────────▼────────┐             ┌────────▼────────┐
│ Galo.Application│◄───────────│Galo.Infrastructure│           │   Galo.Estudos   │
│ Commands/Queries│  implementa│ EF Core (2 ctx)  │ (laboratório)│ EfCoreLab, Cache │
│ Event handlers  │ interfaces │ Redis · RabbitMQ │             │ Concorrencia ... │
└───────┬─────────┘            └──────────────────┘             └──────────────────┘
        │ referencia
┌───────▼─────────┐
│   Galo.Domain   │  entidades · value objects · eventos · domain services (sem dependência de infra)
└─────────────────┘
```

| Projeto | Responsabilidade |
|---|---|
| `Galo.Domain` | Regras de negócio. Agregados, VOs, eventos de domínio, interfaces de repositório. Só depende do MediatR (`INotification`). |
| `Galo.Application` | Casos de uso: **Commands** (escrita) e **Queries** (leitura) via MediatR, handlers de eventos, contratos (`IUnitOfWork`, `ICacheStore`, `IIntegrationEventPublisher`). |
| `Galo.Infrastructure` | EF Core/PostgreSQL, repositórios, Redis (cache 2 níveis), RabbitMQ (publisher + consumidor), health checks, seed. |
| `Galo.Estudos` | Laboratório de estudos (EF, paralelismo, memória, runtime, processamento, cache, concorrência). |
| `Galo.Presentation` | Web API, autenticação, observabilidade, composição de DI. |
| `Conteudo/` | Material teórico de DDD que acompanha o projeto original (Domain-DrivenDesign). |

### Bounded contexts e agregados

| Contexto | Agregados (raiz → filhos) | Destaques |
|---|---|---|
| **Elenco** | `Jogador` → `Contrato` | VOs (`NomeJogador`, `NumeroCamisa`, `ValorMercado`), regra de contratos sem sobreposição |
| **Competicoes** | `Clube`, `Estadio`, `Campeonato`, `Partida` → `GolPartida`, `SetorPartida` | VO `Placar` (OwnsOne); `SetorPartida` guarda o **estoque de ingressos** com `xmin` (concorrência otimista) |
| **Torcida** | `Torcedor` → `Endereco` | VOs `Cpf`, `Email`, `Telefone`, `NomeCompleto`; plano de sócio |
| **Ingressos** | `CompraIngresso` → `ItemIngresso`, `PagamentoIngresso` | máquina de estados; *domain service* de desconto por plano |

Agregados diferentes se referenciam **só por Id** (sem navegação no domínio); o banco tem as FKs.

### CQRS na prática

* **Escrita:** `GaloDbContext` (agregados, tracking, eventos de domínio, `UnitOfWork`).
* **Leitura:** `GaloQueryDbContext` (query models anêmicos nas mesmas tabelas, `NoTracking`, projeção direto para DTO).
* O mesmo modelo de leitura (`QueryModels`) é reutilizado pelo laboratório com *lazy loading proxies*.

### Dois tipos de evento (cai muito em entrevista)

| | Evento de **domínio** | Evento de **integração** |
|---|---|---|
| Exemplo | `PagamentoIniciadoEvent` | `ReservarIngressosIntegrationEvent` |
| Escopo | dentro do processo (MediatR `INotification`) | entre serviços (mensagem JSON no **RabbitMQ**) |
| Quando | logo **após** o `SaveChanges` | assíncrono; pode chegar depois / duplicado |
| Onde nasce | na entidade (`AddDomainEvent`) | num handler de evento de domínio |

### Fluxo de compra de ingressos (consistência eventual)

```
POST /compras                    cria a compra (preço vem da partida; desconto do plano do torcedor)
POST /compras/{id}/pagamentos    IniciarPagamento -> grava -> PagamentoIniciadoEvent (domínio)
                                   └─ handler publica ReservarIngressos no RabbitMQ  (202 Accepted)
        RabbitMQ -> consumidor    ReservarIngressosIntegrationEventHandler
                                   ├─ OK: Partida.ReservarIngressos (xmin + retry) + compra.ConfirmarReserva()
                                   └─ esgotado: cancela a compra (compensação)
POST /compras/{id}/pagamentos/{p}/confirmar   só funciona depois da reserva
POST /compras/{id}/emitir        gera os códigos dos ingressos
POST /compras/{id}/cancelar      se havia reserva, publica LiberarIngressos (devolve o estoque)
```

### Infra: regras respeitadas

* **PostgreSQL:** banco `estudos`; runtime = usuário `app`; migrations = usuário `owner`; nunca `postgres`.
* **Redis:** toda chave/canal com o prefixo `Redis:KeyPrefix` (`estudos:`); **sem** `KEYS/SCAN/FLUSHALL/CONFIG` — a invalidação
  por grupo usa um `SET` índice (`TwoLevelCacheStore`).
* **RabbitMQ:** vhost e usuário `estudos`; publisher com *confirms*, mensagens persistentes, consumidor com `ack` manual.
* **Keycloak (BFF):** Authorization Code + **PKCE (S256)**, cliente confidencial; sessão no servidor (ticket no Redis,
  `RedisTicketStore`); o navegador só recebe um cookie `HttpOnly`/`SameSite=Lax`. Redirect de login
  `http://localhost:4300/api/v1/auth/callback` (o front roda em `:4300` e faz proxy de `/api` para a API em `:5205`).
* **Observabilidade:** logs + traces + métricas por OTLP (http/protobuf), `service.namespace=estudos`;
  métricas próprias em `Galo.Application/Common/Telemetry/GaloTelemetry.cs`.

Endpoints de auth: `GET /api/v1/auth/login?returnUrl=/` · `GET /api/v1/auth/me` · `GET|POST /api/v1/auth/logout`.

---

## 3. O laboratório de estudos — mapa de conceitos

Todos os endpoints são `GET` (exceto onde indicado), funcionam só em Development e devolvem `medicoes` + `observacoes`.
Rode o **seed** antes. Os números são do *seu* computador: use para ordens de grandeza, não como benchmark científico.

| Conceito | Endpoint (`/api/v1/estudos/...`) | O que você vê | Código |
|---|---|---|---|
| **Tracking x NoTracking** | `ef/tracking` | memória e entidades rastreadas | `EfCoreLab.TrackingAsync` |
| **Lazy x Eager x Explicit x Projeção (N+1)** | `ef/carregamento` | **nº de comandos SQL** (lazy 71, explicit 71, eager 1, split 2, projeção 1) | `EfCoreLab.CarregamentoAsync`, `EstudosDbContext` |
| **Projeção, Count x Any, query compilada** | `ef/projecao` | `ToList().Count` x `CountAsync` | `EfCoreLab.ProjecaoAsync` |
| **Operações em massa** | `ef/operacoes-em-massa` | `SaveChanges` x `ExecuteUpdate` (com rollback) | `EfCoreLab.OperacoesEmMassaAsync` |
| **Paginação offset x keyset** | `ef/paginacao` | custo de `Skip` | `EfCoreLab.PaginacaoAsync` |
| **Paralelismo I/O-bound** | `paralelismo/io` | sequencial x `WhenAll` x `Parallel.ForEachAsync`; erro do DbContext compartilhado | `ParalelismoLab.IoBoundAsync` |
| **Paralelismo CPU-bound** | `paralelismo/cpu` | `for` x `Parallel.For` x PLINQ | `ParalelismoLab.CpuBoundAsync` |
| **Race condition** | `paralelismo/corrida` | `++` perde milhões de incrementos; `Interlocked`; `lock` | `ParalelismoLab.CondicaoDeCorridaAsync` |
| **ThreadPool** | `paralelismo/threadpool` | threads, fila, starvation | `ParalelismoLab.ThreadPoolInfo` |
| **Em memória: banco x cliente** | `memoria/servidor-vs-cliente` | `IQueryable` x `IEnumerable` | `MemoriaLab.ServidorVsClienteAsync` |
| **Estruturas de dados** | `memoria/colecoes` | `List` x `HashSet` x `Dictionary` | `MemoriaLab.ColecoesAsync` |
| **Alocação / GC** | `memoria/strings` | `+=` x `StringBuilder`; coletas gen0/1/2 | `MemoriaLab.StringsAsync` |
| **Span, stackalloc, ArrayPool** | `memoria/span` | alocação zero | `MemoriaLab.SpanEPoolAsync` |
| **LINQ x laço** | `memoria/linq` | materialização desnecessária | `MemoriaLab.LinqVsLacoAsync` |
| **Em runtime: reflection x delegate x expression x dynamic** | `runtime/chamadas` | custo por chamada | `RuntimeLab.ChamadasAsync` |
| **Expression Trees -> SQL** | `runtime/filtro-dinamico?campo=Camisa&operador=gt&valor=10` | árvore montada em runtime + **SQL gerado** | `RuntimeLab.FiltroDinamicoAsync` |
| **Source generator x reflection (JSON)** | `runtime/serializacao` | `LabJsonContext` | `RuntimeLab.SerializacaoAsync` |
| **CLR / GC / JIT** | `runtime/info` | modo do GC, pausas, tiered compilation | `RuntimeLab.InfoRuntime` |
| **Channels / backpressure** | `processamento/channels` | pico da fila ilimitada x limitada | `ProcessamentoLab.ChannelsAsync` |
| **Streaming x carregar tudo** | `processamento/streaming` | `AsAsyncEnumerable` | `ProcessamentoLab.StreamingAsync` |
| **Processamento em segundo plano** | `POST processamento/jobs?tipo=simulado&parametro=30` → `GET processamento/jobs/{id}` → `DELETE` p/ cancelar | progresso e cancelamento | `Jobs/FilaDeJobs`, `Jobs/JobWorker` (BackgroundService + Channel) |
| **Cache: sem x L1 x L2 x 2 níveis** | `cache/comparacao` | memória x Redis x banco | `CacheLab`, `TwoLevelCacheStore` |
| **Cache stampede** | `cache/stampede` | 50 execuções do factory x **1** | `CacheLab.StampedeAsync` |
| **Invalidação de cache por grupo** | `cache/invalidacao` | `SET` índice, sem `KEYS/SCAN` | `CacheLab.InvalidacaoAsync` |
| **Competição: 5 estratégias** | `concorrencia/estrategias?capacidade=20&compradores=100` | ingênua **vende 100 para 20 vagas**; lock local; otimista (xmin); pessimista (`FOR UPDATE`); `UPDATE` atômico | `ConcorrenciaLab.EstrategiasAsync` |
| **Conflito otimista passo a passo** | `concorrencia/conflito` | `DbUpdateConcurrencyException` | `ConcorrenciaLab.ConflitoPassoAPassoAsync` |
| **Fluxo completo assíncrono** | `concorrencia/fluxo-completo?capacidade=10&compradores=40` | API → RabbitMQ → consumidor → Postgres, com compensação | `ConcorrenciaLab.FluxoCompletoAsync` |

Também medem-se, nos próprios endpoints de negócio: classificação do campeonato
(`GET /api/v1/campeonatos/{id}/classificacao`) — a 1ª chamada vai ao banco; as seguintes vêm do cache (~2 ms) até uma partida encerrar.

### Roteiro sugerido

1. `POST seed` → `GET ef/carregamento` (entenda o N+1 olhando `comandosSql`) → `GET ef/tracking`.
2. `GET paralelismo/corrida` → `GET concorrencia/estrategias` (o mesmo bug — *lost update* — em memória e no banco).
3. `GET cache/comparacao` → `GET cache/stampede` → `GET campeonatos/{id}/classificacao` duas vezes, encerre uma partida, repita.
4. `POST` compra → pagar → `GET` compra (veja `reservaConfirmada` mudar sozinha) → `GET concorrencia/fluxo-completo`.
5. `GET runtime/filtro-dinamico?...` (veja o SQL) e `POST processamento/jobs`.
6. Abra o `/metrics` e procure `galo_*`; acompanhe os traces no seu coletor OTLP.

---

## 4. Decisões, limitações e próximos passos

* **Outbox:** a publicação no RabbitMQ acontece depois do commit; se a API cair no meio, o evento se perde. A correção é o
  padrão *Outbox* (gravar o evento na mesma transação e despachar por um worker). Está comentado em `GaloDbContext` e no publisher.
* **Reserva de ingresso:** `Partida` (estoque) e `CompraIngresso` são gravadas na mesma transação para nunca divergirem — um
  pragmatismo consciente; a alternativa pura é uma *Saga*.
* **`Atualizar()` dos repositórios é vazio:** a entidade já está rastreada (Unit of Work). Chamar `Update()` num grafo marcaria
  filhos novos como `Modified`.
* **MediatR 14** exige licença comercial em produção (log de aviso em Development). O projeto original já usava a v14; alternativas
  gratuitas: MediatR 12.x ou um mediator por *source generator*.
* **FluentAssertions 7.2** (Apache 2.0): a v8+ tem licença comercial.
* **Laboratório:** `alocadoKb` nas medições assíncronas é aproximado (soma de todas as threads). Para benchmark real use BenchmarkDotNet.
* **Autenticação:** testada até o redirecionamento ao Keycloak (PAR aceito). O fluxo completo exige navegador em `http://localhost:4300`.
* **Ideias:** testes de integração (`WebApplicationFactory<Program>` + Testcontainers), `Hangfire`/`Quartz` para jobs duráveis,
  idempotency-key nos comandos, paginação por cursor na API, *rate limiting*, políticas por papel no Keycloak.
