using Galo.Application.Common.Exceptions;
using Galo.Application.Common.Models;
using Galo.Application.Ingressos.Interfaces;
using MediatR;

namespace Galo.Application.Ingressos.Queries
{
    public sealed record ItemCompraDto(string Setor, int Quantidade, decimal PrecoUnitario, decimal PorcentagemDesconto, decimal ValorTotal);

    public sealed record PagamentoDto(Guid Id, string Metodo, string Status, decimal Valor, string? CodigoTransacao, DateTime? DataPagamento);

    public sealed record CompraResumoDto(Guid Id, string Numero, Guid PartidaId, string Status, decimal ValorTotal, DateTime DataCriacao);

    public sealed record CompraDetalheDto(
        Guid Id, string Numero, Guid TorcedorId, Guid PartidaId, string Status, bool ReservaConfirmada,
        string? MotivoCancelamento, decimal ValorTotal, DateTime DataCriacao,
        IReadOnlyList<ItemCompraDto> Itens, IReadOnlyList<PagamentoDto> Pagamentos);

    public sealed record ObterCompraQuery(Guid Id) : IRequest<CompraDetalheDto>;

    public sealed class ObterCompraQueryHandler : IRequestHandler<ObterCompraQuery, CompraDetalheDto>
    {
        private readonly ICompraQueryRepository _repository;
        public ObterCompraQueryHandler(ICompraQueryRepository repository) => _repository = repository;

        public async Task<CompraDetalheDto> Handle(ObterCompraQuery request, CancellationToken cancellationToken)
            => await _repository.ObterDetalheAsync(request.Id, cancellationToken)
               ?? throw new NotFoundException("Compra", request.Id);
    }

    public sealed record ListarComprasDoTorcedorQuery(Guid TorcedorId, int Pagina = 1, int Tamanho = 20)
        : IRequest<PagedResult<CompraResumoDto>>;

    public sealed class ListarComprasDoTorcedorQueryHandler : IRequestHandler<ListarComprasDoTorcedorQuery, PagedResult<CompraResumoDto>>
    {
        private readonly ICompraQueryRepository _repository;
        public ListarComprasDoTorcedorQueryHandler(ICompraQueryRepository repository) => _repository = repository;

        public Task<PagedResult<CompraResumoDto>> Handle(ListarComprasDoTorcedorQuery request, CancellationToken cancellationToken)
        {
            var (pagina, tamanho) = Paginacao.Normalizar(request.Pagina, request.Tamanho);
            return _repository.ListarDoTorcedorAsync(request.TorcedorId, pagina, tamanho, cancellationToken);
        }
    }
}
