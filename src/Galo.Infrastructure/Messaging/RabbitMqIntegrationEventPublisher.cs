using System.Diagnostics;
using System.Text.Json;
using Galo.Application.Common.Base;
using Galo.Application.Common.Interfaces.IntegrationEvents;
using Galo.Application.Common.Telemetry;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Galo.Infrastructure.Messaging
{
    /// <summary>
    /// Publica eventos de integração no RabbitMQ.
    ///  - Exchange TOPIC durável; routing key = nome do evento (permite filtrar por binding).
    ///  - Mensagem PERSISTENTE + PUBLISHER CONFIRMS: só retornamos depois que o broker confirmou que gravou.
    ///  - Propaga o contexto de trace (traceparent) para o span do consumidor ficar ligado ao do produtor.
    ///
    /// Limitação (ensinada de propósito): a publicação acontece DEPOIS do commit do banco. Se o broker estiver fora,
    /// o dado fica gravado mas o evento se perde. A solução é o padrão OUTBOX (tabela de eventos gravada na mesma
    /// transação + um worker que publica).
    /// </summary>
    public sealed class RabbitMqIntegrationEventPublisher : IIntegrationEventPublisher
    {
        internal static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

        private readonly RabbitMqConnection _connection;
        private readonly ILogger<RabbitMqIntegrationEventPublisher> _logger;
        private static int _exchangeDeclarado;

        public RabbitMqIntegrationEventPublisher(RabbitMqConnection connection, ILogger<RabbitMqIntegrationEventPublisher> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            var nome = integrationEvent.GetType().Name;

            if (!_connection.Options.Habilitado)
            {
                _logger.LogInformation("Mensageria desabilitada; evento {Evento} ({Id}) descartado", nome, integrationEvent.Id);
                return;
            }

            using var activity = GaloTelemetry.Source.StartActivity($"rabbitmq publish {nome}", ActivityKind.Producer);
            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination.name", _connection.Options.Exchange);
            activity?.SetTag("messaging.rabbitmq.destination.routing_key", nome);

            // Serializa pelo tipo CONCRETO (e não por T) para não perder propriedades quando T é a base.
            var corpo = JsonSerializer.SerializeToUtf8Bytes(integrationEvent, integrationEvent.GetType(), Json);

            var conexao = await _connection.ObterAsync(cancellationToken);
            await using var canal = await conexao.CreateChannelAsync(
                new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true),
                cancellationToken);

            if (Interlocked.Exchange(ref _exchangeDeclarado, 1) == 0)
                await canal.ExchangeDeclareAsync(_connection.Options.Exchange, ExchangeType.Topic, durable: true,
                    cancellationToken: cancellationToken);

            var props = new BasicProperties
            {
                Persistent = true,                      // delivery-mode 2: sobrevive a restart do broker (fila durável)
                ContentType = "application/json",
                MessageId = integrationEvent.Id.ToString(),
                Type = nome,
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                Headers = new Dictionary<string, object?>()
            };
            if (activity?.Id is { } traceparent) props.Headers["traceparent"] = traceparent;

            await canal.BasicPublishAsync(_connection.Options.Exchange, nome, mandatory: false, props, corpo, cancellationToken);

            _logger.LogInformation("Evento {Evento} publicado ({Id})", nome, integrationEvent.Id);
        }
    }
}
