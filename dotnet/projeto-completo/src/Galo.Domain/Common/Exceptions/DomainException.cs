namespace Galo.Domain.Common.Exceptions
{
    /// <summary>Violação de regra de negócio. A API traduz para HTTP 400/422.</summary>
    public class DomainException : Exception
    {
        public DomainException() { }
        public DomainException(string message) : base(message) { }
        public DomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}
