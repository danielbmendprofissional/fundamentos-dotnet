namespace Onion.Application.Common.Base
{
    public record Filter
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;

        public int Skip => (Page <= 0 ? 0 : (Page - 1) * PageSize);
        public int Take => PageSize;
    }
}