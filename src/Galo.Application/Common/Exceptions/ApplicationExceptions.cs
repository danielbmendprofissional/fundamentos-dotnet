namespace Galo.Application.Common.Exceptions
{
    /// <summary>Recurso inexistente. A API traduz para HTTP 404.</summary>
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(string recurso, object id)
            : base($"{recurso} '{id}' não encontrado(a).") { }
    }

    /// <summary>
    /// Conflito de concorrência otimista (alguém gravou a mesma linha antes). A Application não conhece o EF Core:
    /// o UnitOfWork traduz DbUpdateConcurrencyException para esta exceção. A API traduz para HTTP 409.
    /// </summary>
    public sealed class ConcurrencyConflictException : Exception
    {
        public ConcurrencyConflictException(string message, Exception? inner = null) : base(message, inner) { }
    }
}
