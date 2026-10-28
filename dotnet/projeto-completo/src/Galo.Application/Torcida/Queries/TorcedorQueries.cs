using Galo.Application.Common.Exceptions;
using Galo.Application.Common.Models;
using Galo.Application.Torcida.Interfaces;
using Galo.Domain.Torcida.Enums;
using MediatR;

namespace Galo.Application.Torcida.Queries
{
    public sealed record TorcedorResumoDto(Guid Id, string Nome, string Email, string Plano, string Status);

    public sealed record EnderecoDto(Guid Id, string Cep, string Logradouro, string Numero, string Bairro,
        string Cidade, string Estado, string Pais, string? Complemento, bool Principal);

    public sealed record TorcedorDetalheDto(
        Guid Id, string Nome, string Cpf, string Email, string? Telefone, DateOnly DataNascimento,
        string Plano, string Status, IReadOnlyList<EnderecoDto> Enderecos);

    public sealed record ListarTorcedoresQuery(PlanoSocio? Plano = null, int Pagina = 1, int Tamanho = 20)
        : IRequest<PagedResult<TorcedorResumoDto>>;

    public sealed class ListarTorcedoresQueryHandler : IRequestHandler<ListarTorcedoresQuery, PagedResult<TorcedorResumoDto>>
    {
        private readonly ITorcedorQueryRepository _repository;
        public ListarTorcedoresQueryHandler(ITorcedorQueryRepository repository) => _repository = repository;

        public Task<PagedResult<TorcedorResumoDto>> Handle(ListarTorcedoresQuery request, CancellationToken cancellationToken)
        {
            var (pagina, tamanho) = Paginacao.Normalizar(request.Pagina, request.Tamanho);
            return _repository.ListarAsync(request.Plano, pagina, tamanho, cancellationToken);
        }
    }

    public sealed record ObterTorcedorQuery(Guid Id) : IRequest<TorcedorDetalheDto>;

    public sealed class ObterTorcedorQueryHandler : IRequestHandler<ObterTorcedorQuery, TorcedorDetalheDto>
    {
        private readonly ITorcedorQueryRepository _repository;
        public ObterTorcedorQueryHandler(ITorcedorQueryRepository repository) => _repository = repository;

        public async Task<TorcedorDetalheDto> Handle(ObterTorcedorQuery request, CancellationToken cancellationToken)
            => await _repository.ObterDetalheAsync(request.Id, cancellationToken)
               ?? throw new NotFoundException("Torcedor", request.Id);
    }
}
