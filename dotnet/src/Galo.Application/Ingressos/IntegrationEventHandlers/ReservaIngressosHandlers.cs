using Galo.Application.Common.Exceptions;
using Galo.Application.Common.Interfaces.Persistence;
using Galo.Application.Common.Telemetry;
using Galo.Application.Ingressos.IntegrationEvents;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Competicoes.Interfaces;
using Galo.Domain.Ingressos.Interfaces;
using Galo.Domain.Ingressos.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Galo.Application.Ingressos.IntegrationEventHandlers
{
    /// <summary>
    /// CONSUMIDOR da mensagem "ReservarIngressos" (chega do RabbitMQ; o consumidor despacha via MediatR).
    ///
    /// Este é o ponto de COMPETIÇÃO do sistema: N torcedores disputam o mesmo SetorPartida.Disponiveis.
    /// Estratégia: CONCORRÊNCIA OTIMISTA (xmin do PostgreSQL no SetorPartida):
    ///   1. lê o setor (versão X)  2. baixa o estoque em memória (regra do domínio)  3. UPDATE ... WHERE xmin = X
    ///   Se outro consumidor gravou antes => 0 linhas afetadas => ConcurrencyConflictException => relê e tenta de novo.
    /// Compare com as outras estratégias no laboratório: GET /api/v1/estudos/concorrencia.
    ///
    /// IDEMPOTENTE: a mensagem pode ser entregue 2x (at-least-once). Se a reserva já foi confirmada, ignora.
    /// </summary>
    public sealed class ReservarIngressosIntegrationEventHandler : INotificationHandler<ReservarIngressosIntegrationEvent>
    {
        private const int MaximoTentativas = 6;

        private readonly IPartidaRepository _partidas;
        private readonly ICompraIngressoRepository _compras;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReservarIngressosIntegrationEventHandler> _logger;

        public ReservarIngressosIntegrationEventHandler(IPartidaRepository partidas, ICompraIngressoRepository compras,
            IUnitOfWork unitOfWork, ILogger<ReservarIngressosIntegrationEventHandler> logger)
        {
            _partidas = partidas;
            _compras = compras;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(ReservarIngressosIntegrationEvent notification, CancellationToken cancellationToken)
        {
            for (var tentativa = 1; tentativa <= MaximoTentativas; tentativa++)
            {
                try
                {
                    var compra = await _compras.ObterPorIdAsync(notification.CompraId, cancellationToken);
                    if (compra is null)
                    {
                        _logger.LogWarning("Reserva ignorada: compra {CompraId} não existe", notification.CompraId);
                        return;
                    }

                    // idempotência / estado inesperado
                    if (compra.ReservaConfirmada || compra.Status != Galo.Domain.Ingressos.Enums.StatusCompra.AguardandoPagamento)
                    {
                        _logger.LogInformation("Reserva ignorada (compra {CompraId} já processada, status {Status})",
                            compra.Id, compra.Status);
                        return;
                    }

                    var partida = await _partidas.ObterPorIdAsync(notification.PartidaId, cancellationToken)
                                  ?? throw new NotFoundException("Partida", notification.PartidaId);

                    foreach (var item in notification.Itens)
                        partida.ReservarIngressos(item.Setor, item.Quantidade); // DomainException se esgotado

                    compra.ConfirmarReserva();

                    _partidas.Atualizar(partida);
                    _compras.Atualizar(compra);

                    // Pragmatismo: Partida (estoque) e Compra são 2 agregados gravados na MESMA transação para que a
                    // reserva e a flag nunca divirjam. O "purista" separaria com Saga/Outbox - ver Conteudo/.
                    await _unitOfWork.CommitAsync(cancellationToken);

                    _logger.LogInformation("Reserva confirmada para a compra {CompraId} (tentativa {Tentativa})",
                        compra.Id, tentativa);
                    return;
                }
                catch (ConcurrencyConflictException)
                {
                    GaloTelemetry.ConflitosConcorrencia.Add(1);
                    _unitOfWork.DiscardChanges();
                    _logger.LogInformation("Conflito de concorrência ao reservar (compra {CompraId}, tentativa {Tentativa}); relendo...",
                        notification.CompraId, tentativa);
                    await Task.Delay(Random.Shared.Next(5, 40) * tentativa, cancellationToken); // backoff com jitter
                }
                catch (DomainException ex)
                {
                    // Ex.: ingressos esgotados. Compensação: cancela a compra (consistência eventual).
                    _unitOfWork.DiscardChanges();
                    _logger.LogWarning("Reserva negada para a compra {CompraId}: {Motivo}", notification.CompraId, ex.Message);
                    await CancelarCompraAsync(notification.CompraId, cancellationToken);
                    return;
                }
            }

            _logger.LogError("Reserva da compra {CompraId} esgotou as {Max} tentativas por conflito de concorrência",
                notification.CompraId, MaximoTentativas);
            await CancelarCompraAsync(notification.CompraId, cancellationToken);
        }

        private async Task CancelarCompraAsync(Guid compraId, CancellationToken cancellationToken)
        {
            var compra = await _compras.ObterPorIdAsync(compraId, cancellationToken);
            if (compra is null || compra.Status == Galo.Domain.Ingressos.Enums.StatusCompra.Cancelada) return;

            compra.Cancelar(MotivoCancelamento.IngressosEsgotados());
            _compras.Atualizar(compra);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }

    /// <summary>Devolve o estoque de uma compra cancelada (também com retry de concorrência).</summary>
    public sealed class LiberarIngressosIntegrationEventHandler : INotificationHandler<LiberarIngressosIntegrationEvent>
    {
        private const int MaximoTentativas = 6;

        private readonly IPartidaRepository _partidas;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LiberarIngressosIntegrationEventHandler> _logger;

        public LiberarIngressosIntegrationEventHandler(IPartidaRepository partidas, IUnitOfWork unitOfWork,
            ILogger<LiberarIngressosIntegrationEventHandler> logger)
        {
            _partidas = partidas;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(LiberarIngressosIntegrationEvent notification, CancellationToken cancellationToken)
        {
            for (var tentativa = 1; tentativa <= MaximoTentativas; tentativa++)
            {
                try
                {
                    var partida = await _partidas.ObterPorIdAsync(notification.PartidaId, cancellationToken);
                    if (partida is null) return;

                    foreach (var item in notification.Itens)
                        partida.LiberarIngressos(item.Setor, item.Quantidade);

                    _partidas.Atualizar(partida);
                    await _unitOfWork.CommitAsync(cancellationToken);
                    return;
                }
                catch (ConcurrencyConflictException)
                {
                    GaloTelemetry.ConflitosConcorrencia.Add(1);
                    _unitOfWork.DiscardChanges();
                    await Task.Delay(Random.Shared.Next(5, 40) * tentativa, cancellationToken);
                }
                catch (DomainException ex)
                {
                    _unitOfWork.DiscardChanges();
                    _logger.LogWarning("Liberação ignorada para a compra {CompraId}: {Motivo}", notification.CompraId, ex.Message);
                    return;
                }
            }

            _logger.LogError("Liberação da compra {CompraId} falhou após {Max} tentativas", notification.CompraId, MaximoTentativas);
        }
    }
}
