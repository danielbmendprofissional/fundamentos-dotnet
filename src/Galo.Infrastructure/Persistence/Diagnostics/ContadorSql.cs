using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Galo.Infrastructure.Persistence.Diagnostics
{
    /// <summary>
    /// Conta quantos comandos SQL o EF executou dentro de um trecho de código. É a ferramenta que torna o
    /// problema N+1 VISÍVEL: "esta tela fez 1 consulta ou 201?".
    ///
    /// Como funciona: um AsyncLocal guarda o contador do "fluxo assíncrono atual". O interceptor (registrado nos
    /// DbContexts) incrementa o contador do fluxo em que o comando foi disparado. Tarefas paralelas criadas dentro do
    /// trecho herdam o mesmo contador (AsyncLocal "flui" com o ExecutionContext), por isso usamos Interlocked.
    /// </summary>
    public sealed class ContadorSql : IDisposable
    {
        private static readonly AsyncLocal<ContadorSql?> _atual = new();
        private int _comandos;
        private readonly List<string> _amostras = new();

        public int Comandos => Volatile.Read(ref _comandos);

        public IReadOnlyList<string> Amostras
        {
            get { lock (_amostras) return _amostras.ToList(); }
        }

        public static ContadorSql Iniciar()
        {
            var contador = new ContadorSql();
            _atual.Value = contador;
            return contador;
        }

        internal static void Registrar(string sql)
        {
            var contador = _atual.Value;
            if (contador is null) return;

            Interlocked.Increment(ref contador._comandos);
            lock (contador._amostras)
            {
                if (contador._amostras.Count < 5)
                    contador._amostras.Add(sql.ReplaceLineEndings(" ").Trim() is { Length: > 220 } s ? s[..220] + "..." : sql.ReplaceLineEndings(" ").Trim());
            }
        }

        public void Dispose() => _atual.Value = null;
    }

    /// <summary>
    /// INTERCEPTOR do EF Core: enxerga todo comando ANTES de ir ao banco. Além de contar, serviria para
    /// logar queries lentas, adicionar hints, auditoria etc.
    /// </summary>
    public sealed class ContadorSqlInterceptor : DbCommandInterceptor
    {
        public static readonly ContadorSqlInterceptor Instancia = new();

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        { ContadorSql.Registrar(command.CommandText); return result; }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData,
            InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        { ContadorSql.Registrar(command.CommandText); return new(result); }

        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        { ContadorSql.Registrar(command.CommandText); return result; }

        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData,
            InterceptionResult<int> result, CancellationToken cancellationToken = default)
        { ContadorSql.Registrar(command.CommandText); return new(result); }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        { ContadorSql.Registrar(command.CommandText); return result; }

        public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData,
            InterceptionResult<object> result, CancellationToken cancellationToken = default)
        { ContadorSql.Registrar(command.CommandText); return new(result); }
    }
}
