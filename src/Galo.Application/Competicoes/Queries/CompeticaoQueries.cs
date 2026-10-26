using Galo.Application.Common.Exceptions;
using Galo.Application.Common.Interfaces.Caching;
using Galo.Application.Common.Models;
using Galo.Application.Competicoes.Interfaces;
using Galo.Domain.Competicoes.Enums;
using MediatR;

namespace Galo.Application.Competicoes.Queries
{
    public sealed record ClubeDto(Guid Id, string Nome, string Sigla, string Cidade, string Uf, bool EhOGalo);
    public sealed record EstadioDto(Guid Id, string Nome, string Cidade, int Capacidade);
    public sealed record CampeonatoDto(Guid Id, string Nome, int Temporada, string Tipo, string Status);

    public sealed record PartidaResumoDto(
        Guid Id, string Campeonato, int Rodada, string Mandante, string Visitante,
        DateTime DataHoraUtc, string Status, int GolsMandante, int GolsVisitante);

    public sealed record GolDto(int Minuto, string Clube, string? Jogador, string Tipo);
    public sealed record SetorDto(string Nome, int Capacidade, int Disponiveis, decimal Preco);

    public sealed record PartidaDetalheDto(
        Guid Id, string Campeonato, int Rodada, string Estadio, string Mandante, string Visitante,
        DateTime DataHoraUtc, string Status, int GolsMandante, int GolsVisitante,
        IReadOnlyList<GolDto> Gols, IReadOnlyList<SetorDto> Setores);

    public sealed record ClassificacaoItemDto(
        int Posicao, Guid ClubeId, string Clube, string Sigla, int Jogos, int Vitorias, int Empates,
        int Derrotas, int GolsPro, int GolsContra, int SaldoGols, int Pontos);

    // ---------------- Partidas ----------------

    public sealed record ListarPartidasQuery(
        Guid? CampeonatoId = null, Guid? ClubeId = null, StatusPartida? Status = null, int Pagina = 1, int Tamanho = 20)
        : IRequest<PagedResult<PartidaResumoDto>>;

    public sealed class ListarPartidasQueryHandler : IRequestHandler<ListarPartidasQuery, PagedResult<PartidaResumoDto>>
    {
        private readonly IPartidaQueryRepository _repository;
        public ListarPartidasQueryHandler(IPartidaQueryRepository repository) => _repository = repository;

        public Task<PagedResult<PartidaResumoDto>> Handle(ListarPartidasQuery request, CancellationToken cancellationToken)
        {
            var (pagina, tamanho) = Paginacao.Normalizar(request.Pagina, request.Tamanho);
            return _repository.ListarAsync(request.CampeonatoId, request.ClubeId, request.Status, pagina, tamanho, cancellationToken);
        }
    }

    public sealed record ObterPartidaQuery(Guid Id) : IRequest<PartidaDetalheDto>;

    public sealed class ObterPartidaQueryHandler : IRequestHandler<ObterPartidaQuery, PartidaDetalheDto>
    {
        private readonly IPartidaQueryRepository _repository;
        public ObterPartidaQueryHandler(IPartidaQueryRepository repository) => _repository = repository;

        public async Task<PartidaDetalheDto> Handle(ObterPartidaQuery request, CancellationToken cancellationToken)
            => await _repository.ObterDetalheAsync(request.Id, cancellationToken)
               ?? throw new NotFoundException("Partida", request.Id);
    }

    // ---------------- Classificação (cacheada) ----------------

    public static class GruposDeCache
    {
        public const string Classificacao = "classificacao";
        public const string Catalogo = "catalogo";
    }

    public sealed record ObterClassificacaoQuery(Guid CampeonatoId) : IRequest<IReadOnlyList<ClassificacaoItemDto>>;

    public sealed class ObterClassificacaoQueryHandler : IRequestHandler<ObterClassificacaoQuery, IReadOnlyList<ClassificacaoItemDto>>
    {
        private readonly IPartidaQueryRepository _repository;
        private readonly ICacheStore _cache;

        public ObterClassificacaoQueryHandler(IPartidaQueryRepository repository, ICacheStore cache)
        {
            _repository = repository;
            _cache = cache;
        }

        // CACHE-ASIDE: tenta o cache; no miss, executa a consulta (cara) e guarda.
        // A chave pertence ao grupo "classificacao": quando uma partida ENCERRA, o handler de evento invalida o grupo.
        public Task<IReadOnlyList<ClassificacaoItemDto>> Handle(ObterClassificacaoQuery request, CancellationToken cancellationToken)
            => _cache.GetOrCreateAsync<IReadOnlyList<ClassificacaoItemDto>>(
                key: $"classificacao:{request.CampeonatoId}",
                factory: ct => _repository.ObterClassificacaoAsync(request.CampeonatoId, ct),
                ttl: TimeSpan.FromMinutes(5),
                group: GruposDeCache.Classificacao,
                cancellationToken: cancellationToken);
    }

    // ---------------- Cadastros (cacheados) ----------------

    public sealed record ListarClubesQuery() : IRequest<IReadOnlyList<ClubeDto>>;
    public sealed record ListarEstadiosQuery() : IRequest<IReadOnlyList<EstadioDto>>;
    public sealed record ListarCampeonatosQuery() : IRequest<IReadOnlyList<CampeonatoDto>>;

    public sealed class ListarCatalogoQueryHandler :
        IRequestHandler<ListarClubesQuery, IReadOnlyList<ClubeDto>>,
        IRequestHandler<ListarEstadiosQuery, IReadOnlyList<EstadioDto>>,
        IRequestHandler<ListarCampeonatosQuery, IReadOnlyList<CampeonatoDto>>
    {
        private readonly ICatalogoQueryRepository _repository;
        private readonly ICacheStore _cache;

        public ListarCatalogoQueryHandler(ICatalogoQueryRepository repository, ICacheStore cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public Task<IReadOnlyList<ClubeDto>> Handle(ListarClubesQuery request, CancellationToken cancellationToken)
            => _cache.GetOrCreateAsync("catalogo:clubes", ct => _repository.ListarClubesAsync(ct),
                TimeSpan.FromMinutes(10), GruposDeCache.Catalogo, cancellationToken);

        public Task<IReadOnlyList<EstadioDto>> Handle(ListarEstadiosQuery request, CancellationToken cancellationToken)
            => _cache.GetOrCreateAsync("catalogo:estadios", ct => _repository.ListarEstadiosAsync(ct),
                TimeSpan.FromMinutes(10), GruposDeCache.Catalogo, cancellationToken);

        public Task<IReadOnlyList<CampeonatoDto>> Handle(ListarCampeonatosQuery request, CancellationToken cancellationToken)
            => _cache.GetOrCreateAsync("catalogo:campeonatos", ct => _repository.ListarCampeonatosAsync(ct),
                TimeSpan.FromMinutes(10), GruposDeCache.Catalogo, cancellationToken);
    }
}
