using Galo.Application.Common.Exceptions;
using Galo.Domain.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Galo.Presentation.Infrastructure
{
    /// <summary>
    /// Tradução CENTRAL de exceções para respostas HTTP (RFC 7807 - ProblemDetails). Os controllers ficam sem try/catch.
    ///   DomainException          => 422 (regra de negócio violada)
    ///   NotFoundException        => 404
    ///   ConcurrencyConflict      => 409 (alguém alterou o mesmo dado)
    ///   violação de unicidade    => 409
    ///   qualquer outra           => 500 SEM detalhes (não vazar stack trace/SQL para o cliente; vai para o log)
    /// </summary>
    public sealed class GaloExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GaloExceptionHandler> _logger;
        public GaloExceptionHandler(ILogger<GaloExceptionHandler> logger) => _logger = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, titulo, detalhe) = exception switch
            {
                DomainException e => (StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada", e.Message),
                NotFoundException e => (StatusCodes.Status404NotFound, "Recurso não encontrado", e.Message),
                ConcurrencyConflictException e => (StatusCodes.Status409Conflict, "Conflito de concorrência", e.Message),
                DbUpdateException { InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } }
                    => (StatusCodes.Status409Conflict, "Registro duplicado", "Já existe um registro com estes dados."),
                DbUpdateException { InnerException: PostgresException { SqlState: PostgresErrorCodes.CheckViolation } }
                    => (StatusCodes.Status409Conflict, "Restrição violada", "A operação violaria uma restrição de integridade."),
                BadHttpRequestException e => (StatusCodes.Status400BadRequest, "Requisição inválida", e.Message),
                _ => (StatusCodes.Status500InternalServerError, "Erro interno", "Ocorreu um erro inesperado.")
            };

            if (status >= 500) _logger.LogError(exception, "Erro não tratado em {Metodo} {Caminho}", httpContext.Request.Method, httpContext.Request.Path);
            else _logger.LogInformation("{Titulo}: {Detalhe}", titulo, detalhe);

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = titulo,
                Detail = detalhe,
                Instance = httpContext.Request.Path
            }, cancellationToken);

            return true;
        }
    }
}
