using System.Collections.Generic;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.Abstractions.Contracts.Auditoria
{
    /// <summary>
    /// Contrato que abstrai uma entrada de entidade rastreada, permitindo o processamento agnóstico ao ORM.
    /// </summary>
    public interface IAuditEntry
    {
        /// <summary>
        /// Referência para a instância real da entidade POCO sendo auditada.
        /// </summary>
        object Entity { get; }

        /// <summary>
        /// Estado atual da entidade no ciclo de vida da persistência (Added, Modified, Deleted).
        /// </summary>
        AuditEntryState State { get; }

        /// <summary>
        /// Coleção de todas as propriedades escalares e de valor da entidade.
        /// </summary>
        IEnumerable<IAuditProperty> Properties { get; }

        /// <summary>
        /// Metadados estruturais do tipo da entidade (ex: EntityType ou IReadOnlyEntityType).
        /// </summary>
        object TypeMetadata { get; }

        /// <summary>
        /// Localiza uma propriedade específica da entidade pelo seu nome.
        /// </summary>
        /// <param name="propertyName">Nome da propriedade a ser localizada.</param>
        /// <returns>Uma implementação de <see cref="IAuditProperty"/>.</returns>
        IAuditProperty Property(string propertyName);

        object Navigation(string navigationName);
    }
}
