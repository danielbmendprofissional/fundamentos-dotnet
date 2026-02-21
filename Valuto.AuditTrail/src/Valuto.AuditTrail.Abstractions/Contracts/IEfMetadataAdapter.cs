using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.Abstractions.Contracts
{
    /// <summary>
    /// Define um adaptador para extração e normalização de metadados do Entity Framework via Reflection.
    /// </summary>
    public interface IEfMetadataAdapter
    {
        /// <summary>
        /// Obtém o nome da propriedade a partir de um objeto de metadados do EF (Property ou Navigation).
        /// </summary>
        string GetPropertyName(object metadata);

        /// <summary>
        /// Localiza e retorna a metadata da PrimaryKey de forma tipada.
        /// </summary>
        object FindPrimaryKey(object typeMetadata);

        /// <summary>
        /// Retorna as propriedades (colunas) contidas em um metadado (Key ou ForeignKey).
        /// </summary>
        IEnumerable GetPropertiesFromMetadata(object metadataContainer);

        /// <summary>
        /// Localiza e retorna as ForeignKeys de um tipo de entidade.
        /// </summary>
        IEnumerable GetForeignKeys(object typeMetadata);

        /// <summary>
        /// Obtém o nome da navegação (Principal ou Dependente) de uma ForeignKey.
        /// </summary>
        string GetNavigationNameFromForeignKey(object fk);

        /// <summary>
        /// Obtém o nome da primeira propriedade (coluna) que compõe uma ForeignKey.
        /// </summary>
        string GetFirstFkPropertyName(object fk);

        /// <summary>
        /// Busca o atributo <see cref="ValutoAuditableAttribute"/> no tipo, tratando Proxies.
        /// </summary>
        ValutoAuditableAttribute GetAuditableAttribute(Type type);

        /// <summary>
        /// Obtém o nome simples da entidade.
        /// </summary>
        string GetEntityName(Type type);

        /// <summary>
        /// Obtém o Namespace original da entidade.
        /// </summary>
        string GetNamespace(Type type);

        /// <summary>
        /// Limpa e normaliza o nome da entidade a partir de metadados brutos.
        /// </summary>
        string GetCleanEntityName(object metadata);

        /// <summary>
        /// Recupera metadados de navegações do tipo coleção (1:N, N:N).
        /// </summary>
        IEnumerable<object> GetCollectionsMetadata(object typeMetadata);

        /// <summary>
        /// Recupera metadados de navegações do tipo referência (1:1, N:1).
        /// </summary>
        IEnumerable<object> GetReferencesMetadata(object typeMetadata);

        /// <summary>
        /// OBRIGATORIEDADE: Recupera o estado de rastreamento de uma instância de forma agnóstica.
        /// </summary>
        AuditEntryState GetEntityState(IAuditTracker tracker, object entity);

        /// <summary>
        /// Extrai o valor atual (CurrentValue) de uma entrada de navegação ou propriedade de forma agnóstica.
        /// </summary>
        /// <param name="navEntry">O objeto de entrada (NavigationEntry ou DbMemberEntry).</param>
        /// <returns>O valor da instância (geralmente uma IEnumerable ou objeto POCO).</returns>
        object GetCurrentValue(object navEntry);

        /// <summary>
        /// Verifica se uma navegação foi modificada de forma agnóstica.
        /// </summary>
        bool IsNavigationModified(object navEntry);
    }
}

