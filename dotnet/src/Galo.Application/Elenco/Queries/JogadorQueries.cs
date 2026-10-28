using Galo.Application.Common.Exceptions;
using Galo.Application.Common.Models;
using Galo.Application.Elenco.Interfaces;
using Galo.Domain.Elenco.Enums;
using MediatR;

namespace Galo.Application.Elenco.Queries
{
    // QUERY = pergunta: não muda estado, devolve DTO. Não usa repositório de domínio nem UnitOfWork.

    public sealed record JogadorResumoDto(
        Guid Id, string Nome, string? Apelido, int Camisa, string Posicao, string Status, decimal ValorMercado);

    public sealed record ContratoDto(Guid Id, DateOnly Inicio, DateOnly Fim, decimal SalarioMensal, decimal? ClausulaRescisoria);

    public sealed record JogadorDetalheDto(
        Guid Id, string Nome, string? Apelido, int Camisa, string Posicao, string Status,
        DateOnly DataNascimento, string Nacionalidade, decimal ValorMercado,
        IReadOnlyList<ContratoDto> Contratos);

    public sealed record ListarJogadoresQuery(
        Posicao? Posicao = null, StatusJogador? Status = null, int Pagina = 1, int Tamanho = 20)
        : IRequest<PagedResult<JogadorResumoDto>>;

    public sealed class ListarJogadoresQueryHandler : IRequestHandler<ListarJogadoresQuery, PagedResult<JogadorResumoDto>>
    {
        private readonly IJogadorQueryRepository _repository;
        public ListarJogadoresQueryHandler(IJogadorQueryRepository repository) => _repository = repository;

        public Task<PagedResult<JogadorResumoDto>> Handle(ListarJogadoresQuery request, CancellationToken cancellationToken)
        {
            var (pagina, tamanho) = Paginacao.Normalizar(request.Pagina, request.Tamanho);
            return _repository.ListarAsync(request.Posicao, request.Status, pagina, tamanho, cancellationToken);
        }
    }

    public sealed record ObterJogadorQuery(Guid Id) : IRequest<JogadorDetalheDto>;

    public sealed class ObterJogadorQueryHandler : IRequestHandler<ObterJogadorQuery, JogadorDetalheDto>
    {
        private readonly IJogadorQueryRepository _repository;
        public ObterJogadorQueryHandler(IJogadorQueryRepository repository) => _repository = repository;

        public async Task<JogadorDetalheDto> Handle(ObterJogadorQuery request, CancellationToken cancellationToken)
            => await _repository.ObterDetalheAsync(request.Id, cancellationToken)
               ?? throw new NotFoundException("Jogador", request.Id);
    }
}
