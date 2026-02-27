using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis.System.Diagnostics.CodeAnalysis;
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
        [AllowNull]
        object? Entity { get; }

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
        [AllowNull]
        object? TypeMetadata { get; }

        /// <summary>
        /// Localiza uma propriedade específica da entidade pelo seu nome.
        /// </summary>
        /// <param name="propertyName">Nome da propriedade a ser localizada.</param>
        /// <returns>Uma implementação de <see cref="IAuditProperty"/>.</returns>
        [return: MaybeNull]
        IAuditProperty? Property(string propertyName);

        /// <summary>
        /// Obtém o estado da navegação (coleção ou referência) para uma propriedade de navegação específica.
        /// </summary>
        /// <param name="navigationName">O nome da propriedade de navegação.</param>
        /// <returns>Um objeto representando os metadados e o estado da navegação da entidade.</returns>
        [return: MaybeNull]
        object? Navigation(string navigationName);
    }
}
