using System.Threading;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Model.Helpers;

namespace Valuto.AuditTrail.Abstractions.Contracts.Auditoria
{
    /// <summary>
    /// Interface com os serviços de auditoria disponíveis
    /// </summary>
    public interface IAuditoriaInterna
    {
        /// <summary>
        /// Inicia o rastro de auditoria de forma sincrona, obtendo informações do ChangeTracker
        /// antes de ser alterado no saveChgens.
        /// </summary>
        /// <param name="ctx">Helper que será preenchido com os valores obtidos durante a execução.</param>
        /// <param name="tracker">ChangeTracker que contém as informações da operação atual do client.</param>
        void IniciarAuditoria(AuditProcessingContextHelper ctx, IAuditTracker tracker);
        /// <summary>
        /// Finaliza o rastro de auditoria de forma assincrona, obtendo informações do ChangeTracker
        /// depois ser alterado no saveChgens.
        /// </summary>
        /// <param name="ctx">Helper que será preenchido com os valores obtidos após a execução.</param>
        /// <param name="cancellationToken">Token de cancelamento para interromper a operação assíncrona.</param>
        Task FinalizarAuditoriaAsync(AuditProcessingContextHelper ctx, CancellationToken cancellationToken);
    }
}
