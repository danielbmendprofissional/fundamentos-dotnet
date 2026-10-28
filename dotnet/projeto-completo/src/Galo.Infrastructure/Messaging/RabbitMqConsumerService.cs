using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Galo.Application.Common.Telemetry;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Galo.Infrastructure.Messaging
{
    /// <summary>
    /// Consumidor (BackgroundService = loop que roda junto com a API).
    /// Para cada mensagem: cria um ESCOPO de DI (cada mensagem = um "request": DbContext próprio),
    /// desserializa e despacha via MediatR para os INotificationHandler do evento. Só então dá ACK.
    ///
    /// Entrega "at-least-once": se o processo cair antes do ACK a mensagem volta => handlers precisam ser IDEMPOTENTES.
    /// </summary>
    public sealed class RabbitMqConsumerService : BackgroundService
    {
        private readonly RabbitMqConnection _connection;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IntegrationEventTypeRegistry _registry;
        private readonly ILogger<RabbitMqConsumerService> _logger;

        public RabbitMqConsumerService(RabbitMqConnection connection, IServiceScopeFactory scopeFactory,
            IntegrationEventTypeRegistry registry, ILogger<RabbitMqConsumerService> logger)
        {
            _connection = connection;
            _scopeFactory = scopeFactory;
            _registry = registry;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_connection.Options.Habilitado) return;

            // Se o broker estiver fora ao subir, não derruba a API: tenta de novo a cada 5 s.
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var conexao = await _connection.ObterAsync(stoppingToken);
                    var canal = await conexao.CreateChannelAsync(cancellationToken: stoppingToken);

                    var o = _connection.Options;
                    await canal.ExchangeDeclareAsync(o.Exchange, ExchangeType.Topic, durable: true, cancellationToken: stoppingToken);
                    await canal.QueueDeclareAsync(o.Queue, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
                    await canal.QueueBindAsync(o.Queue, o.Exchange, routingKey: "#", cancellationToken: stoppingToken);
                    await canal.BasicQosAsync(0, o.Prefetch, false, stoppingToken);

                    var consumidor = new AsyncEventingBasicConsumer(canal);
                    consumidor.ReceivedAsync += (_, ea) => ProcessarAsync(canal, ea, stoppingToken);
                    await canal.BasicConsumeAsync(o.Queue, autoAck: false, consumidor, stoppingToken);

                    _logger.LogInformation("Consumidor RabbitMQ ativo na fila {Fila}", o.Queue);

                    // Fica "preso" aqui enquanto o canal estiver vivo (o auto-recovery do client cuida de quedas curtas).
                    while (!stoppingToken.IsCancellationRequested && canal.IsOpen)
                        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

                    await canal.DisposeAsync();
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
                catch (Exception ex)
                {
                    _logger.LogWarning("RabbitMQ indisponível ({Tipo}: {Mensagem}). Nova tentativa em 5 s",
                        ex.GetType().Name, ex.Message);
                    try { await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); } catch (OperationCanceledException) { }
                }
            }
        }

        private async Task ProcessarAsync(IChannel canal, BasicDeliverEventArgs ea, CancellationToken ct)
        {
            var nome = ea.BasicProperties.Type ?? ea.RoutingKey;

            // Reconstrói o contexto de trace vindo do produtor (se houver)
            ActivityContext.TryParse(LerHeader(ea, "traceparent"), null, out var contextoPai);
            using var activity = GaloTelemetry.Source.StartActivity($"rabbitmq consume {nome}", ActivityKind.Consumer, contextoPai);
            activity?.SetTag("messaging.system", "rabbitmq");

            try
            {
                var tipo = _registry.Resolver(nome);
                if (tipo is null)
                {
                    _logger.LogWarning("Evento desconhecido '{Evento}' descartado", nome);
                    await canal.BasicAckAsync(ea.DeliveryTag, false, ct); // não adianta reentregar o que não entendemos
                    return;
                }

                var evento = JsonSerializer.Deserialize(ea.Body.Span, tipo, RabbitMqIntegrationEventPublisher.Json)!;

                await using var escopo = _scopeFactory.CreateAsyncScope();
                var publisher = escopo.ServiceProvider.GetRequiredService<IPublisher>();
                await publisher.Publish(evento, ct);

                await canal.BasicAckAsync(ea.DeliveryTag, false, ct);
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

                // Primeira falha: recoloca na fila (pode ter sido transitória). Segunda: descarta
                // (sem DLQ configurada, reentregar para sempre travaria a fila: "poison message").
                var reenfileirar = !ea.Redelivered;
                _logger.LogError(ex, "Falha ao processar {Evento} (redelivered={Redelivered}); {Acao}",
                    nome, ea.Redelivered, reenfileirar ? "reenfileirando" : "descartando");
                await canal.BasicNackAsync(ea.DeliveryTag, false, reenfileirar, CancellationToken.None);
            }
        }

        private static string? LerHeader(BasicDeliverEventArgs ea, string chave)
            => ea.BasicProperties.Headers is { } h && h.TryGetValue(chave, out var v)
                ? v switch { byte[] b => Encoding.UTF8.GetString(b), string s => s, _ => null }
                : null;
    }
}
