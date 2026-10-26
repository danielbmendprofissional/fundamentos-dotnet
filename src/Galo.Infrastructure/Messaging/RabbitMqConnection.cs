using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Galo.Infrastructure.Messaging
{
    /// <summary>
    /// Mantém UMA conexão TCP com o RabbitMQ para o processo inteiro (singleton).
    /// Regra de ouro do RabbitMQ.Client: CONEXÃO é cara e compartilhada; CANAL (channel) é leve, NÃO é thread-safe
    /// e deve ser criado por operação/consumidor.
    /// </summary>
    public sealed class RabbitMqConnection : IAsyncDisposable
    {
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqConnection> _logger;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private IConnection? _connection;

        public RabbitMqConnection(IOptions<RabbitMqOptions> options, ILogger<RabbitMqConnection> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public RabbitMqOptions Options => _options;

        public bool EstaConectado => _connection is { IsOpen: true };

        public async Task<IConnection> ObterAsync(CancellationToken cancellationToken = default)
        {
            if (_connection is { IsOpen: true }) return _connection;

            await _lock.WaitAsync(cancellationToken);
            try
            {
                if (_connection is { IsOpen: true }) return _connection;

                var factory = new ConnectionFactory
                {
                    HostName = _options.Host,
                    Port = _options.Port,
                    VirtualHost = _options.VirtualHost,
                    UserName = _options.UserName,
                    Password = _options.Password,
                    ClientProvidedName = "galo-api",
                    AutomaticRecoveryEnabled = true,       // reconecta sozinho se o broker cair
                    TopologyRecoveryEnabled = true,        // e refaz filas/bindings/consumers
                    RequestedHeartbeat = TimeSpan.FromSeconds(30)
                };

                _connection = await factory.CreateConnectionAsync(cancellationToken);
                _logger.LogInformation("Conectado ao RabbitMQ (vhost {VHost})", _options.VirtualHost);
                return _connection;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection is not null)
            {
                try { await _connection.CloseAsync(); } catch { /* encerrando */ }
                _connection.Dispose();
            }
            _lock.Dispose();
        }
    }
}
