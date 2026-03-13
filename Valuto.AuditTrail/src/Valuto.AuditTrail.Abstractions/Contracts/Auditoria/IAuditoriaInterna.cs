using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Model.Helpers;
using Valuto.AuditTrail.Abstractions.Model.Snapshots;

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
        /// <param name="tracker">ChangeTracker que contém as informações da operação atual do client.</param>
        (List<EntitySnapshot> Snapshots, List<Action> Materializadores) GerarSnapshot(ITrackerAdapter tracker);
        /// <summary>
        /// Finaliza o rastro de auditoria de forma assincrona, obtendo informações do ChangeTracker
        /// depois ser alterado no saveChgens.
        /// </summary>
        Task ProcessarAuditoriaAsync(List<EntitySnapshot> snapshots, CancellationToken cancellationToken = default);
    }
}
