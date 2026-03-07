using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots
{
    /// <summary>
    /// Representa um snapshot imutável do estado de uma entidade rastreada para fins de auditoria.
    /// </summary>
    public sealed class AuditEntrySnapshot : IAuditEntry
    {
        /// <summary>
        /// Obtém ou define a instância da entidade auditada. 
        /// O uso de [AllowNull] indica que, embora o contrato prefira um objeto, 
        /// cenários de deleção ou desconexão podem resultar em nulo.
        /// </summary>
        [AllowNull]
        public object? Entity { get; set; }

        /// <summary>
        /// Obtém ou define o estado operacional da entidade (Added, Modified, Deleted).
        /// </summary>
        public AuditEntryState State { get; set; }

        /// <summary>
        /// Enumeração de propriedades auditadas. 
        /// OBRIGATORIEDADE: Sempre retornará uma coleção, mesmo que vazia, para evitar NullReference.
        /// </summary>
        public IEnumerable<IAuditProperty> Properties { get; set; } = Enumerable.Empty<IAuditProperty>();

        /// <summary>
        /// Obtém ou define metadados adicionais do tipo da entidade (ex: Reflection ou EF Metadata).
        /// </summary>
        [AllowNull]
        public object? TypeMetadata { get; set; }

        /// <summary>
        /// Dicionário contendo entradas de navegação (relacionamentos) capturadas no snapshot.
        /// </summary>
        public Dictionary<string, object> NavEntries { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Localiza uma propriedade específica pelo nome dentro do snapshot.
        /// </summary>
        /// <param name="name">O nome da propriedade cadastrada no mapeamento.</param>
        /// <returns>A instância de <see cref="IAuditProperty"/> ou nulo caso não encontrada.</returns>
        [return: MaybeNull]
        public IAuditProperty? Property(string name) => Properties.FirstOrDefault(p => p.Name == name);

        /// <summary>
        /// Recupera um objeto de navegação (relacionamento) pelo nome.
        /// </summary>
        /// <param name="name">O nome da propriedade de navegação.</param>
        /// <returns>O objeto de navegação correspondente ou nulo.</returns>
        [return: MaybeNull]
        public object? Navigation(string name) => NavEntries.TryGetValue(name, out var nav) ? nav : null;
    }
}
