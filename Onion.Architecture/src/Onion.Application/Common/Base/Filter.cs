namespace Onion.Application.Common.Base
{
    public record Filter(
        int Page,
        int PageSize,
        int Skip,
        int Take);
}