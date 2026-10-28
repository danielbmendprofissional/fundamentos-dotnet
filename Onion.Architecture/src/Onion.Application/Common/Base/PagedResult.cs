namespace Onion.Application.Common.Base
{
    public record PagedResult<T>(IEnumerable<T> Items, int Total, int Page, int PageSize);
}