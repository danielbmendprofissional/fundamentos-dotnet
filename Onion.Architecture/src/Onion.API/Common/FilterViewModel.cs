namespace Onion.API.Common
{
    public record FilterViewModel
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;

        public int Skip => (Page <= 0 ? 0 : (Page - 1) * PageSize);
        public int Take => PageSize;
    }
}
