using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria.Adapters;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.Abstractions.Contracts.Auditoria
{
    /// <summary>
    /// Contrato que abstrai uma entrada de entidade rastreada, permitindo o processamento agnóstico ao ORM.
    /// </summary>
    public interface IEntityAdapter
    {
        public Type? Type { get; }
        public string? Name { get; }
        public string? Namespace { get; }

        /// <summary>
        /// Estado atual da entidade no ciclo de vida da persistência (Added, Modified, Deleted).
        /// </summary>
        EntryState State { get; }

        /// <summary>
        /// Coleção de todas as propriedades escalares e de valor da entidade.
        /// </summary>
        IEnumerable<IPropertyAdapter> Properties { get; }

        IEnumerable<IForeignKeyAdapter> ForeignKeys { get; }

        IEnumerable<INavigationAdapter> Navigations { get; }

        IEnumerable<IReferenceAdapter> References { get; }
    }
}
