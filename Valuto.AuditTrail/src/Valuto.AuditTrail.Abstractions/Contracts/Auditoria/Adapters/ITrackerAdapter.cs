using System.Collections.Generic;

namespace Valuto.AuditTrail.Abstractions.Contracts.Auditoria
{
    /// <summary>
    /// Abstração do mecanismo de rastreamento de alterações (Change Tracker) do fornecedor de dados.
    /// </summary>
    public interface ITrackerAdapter
    {
        /// <summary>
        /// Recupera todas as entradas de entidades que estão sob gestão do rastreador de alterações.
        /// </summary>
        /// <returns>Uma coleção de <see cref="IEntityAdapter"/> representando as entidades modificadas.</returns>
        IEnumerable<IEntityAdapter> Entries(IMetadataAdapter metadataAdapter);
    }
}
