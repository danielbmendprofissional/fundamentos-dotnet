using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Entities;
using Valuto.AuditTrail.Abstractions.Model.Snapshots;

namespace Valuto.AuditTrail.Abstractions.Model.Helpers
{
    /// <summary>
    /// Classe de suporte que vincula uma entrada rastreada à sua respectiva trilha de auditoria.
    /// </summary>
    public sealed class AuditHelper
    {
        /// <summary>
        /// Obtém a entrada da entidade abstraída.
        /// </summary>
        /// <remarks>
        /// Sempre será uma interface <see cref="EntitySnapshot"/> para garantir independência do ORM.
        /// </remarks>
        public EntitySnapshot Entry { get; }

        /// <summary>
        /// Obtém a entidade de trilha de auditoria que armazena os valores para persistência.
        /// </summary>
        public AuditTrails Trail { get; }

        /// <summary>
        /// Inicializa uma nova instância de <see cref="AuditHelper"/>.
        /// </summary>
        /// <param name="entry">A entrada abstraída da entidade.</param>
        /// <param name="trail">A trilha de auditoria vinculada.</param>
        public AuditHelper(EntitySnapshot entry, AuditTrails trail)
        {
            Entry = entry;
            Trail = trail;
        }
    }

}
