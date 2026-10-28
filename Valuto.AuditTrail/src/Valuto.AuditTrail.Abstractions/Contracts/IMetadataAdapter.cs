using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.DTOs;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.Abstractions.Contracts
{
    /// <summary>
    /// Define um adaptador para extração e normalização de metadados do Entity Framework via Reflection.
    /// </summary>
    public interface IMetadataAdapter
    {
        [return: MaybeNull]
        IEnumerable<PropertyMetadata> GetProperties(object metadata);

        /// <summary>
        /// Localiza e retorna as ForeignKeys de um tipo de entidade.
        /// </summary>
        [return: MaybeNull]
        IEnumerable<ForeignKeyMetadata>? GetForeignKeys(object metadata);

        /// <summary>
        /// Recupera metadados de navegações do tipo coleção (1:N, N:N).
        /// </summary>
        IEnumerable<NavigationMetadata> GetNavigations(object metadata);

        string GetPrincipalEntityName(object metadata);

        /// <summary>
        /// Busca o atributo <see cref="ValutoAuditableAttribute"/> no tipo, tratando Proxies.
        /// </summary>
        [return: MaybeNull]
        ValutoAuditableAttribute? GetAuditableAttribute(Type type);

        /// <summary>
        /// Obtém o Namespace original da entidade.
        /// </summary>
        string GetNamespace(Type type);
    }
}

