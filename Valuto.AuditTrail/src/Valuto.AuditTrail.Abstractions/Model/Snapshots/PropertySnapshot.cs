using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots
{
    /// <summary>
    /// Representa o estado capturado de uma propriedade individual para a trilha de auditoria.
    /// Implementa suporte a valores temporários (Lazy Loading) para chaves geradas pelo banco de dados.
    /// </summary>
    public sealed class PropertySnapshot
    {
        /// <summary>
        /// Obtém ou define o nome técnico da propriedade no mapeamento do Entity Framework.
        /// </summary>
        public string Name { get; } = string.Empty;

        /// <summary>
        /// Obtém o valor atual da propriedade. 
        /// </summary>
        public object? CurrentValue { get; private set; }

        /// <summary>
        /// Obtém ou define o valor original da propriedade antes da modificação.
        /// </summary>
        [AllowNull]
        public object? OriginalValue { get; set; }

        /// <summary>
        /// Indica se houve alteração entre o valor original e o valor atual durante o ciclo de vida da transação.
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        /// Indica se o valor é temporário (ex: PKs não persistidas) e requer resolução posterior.
        /// </summary>
        public bool IsTemporary { get; set; }

        public bool IsPrimaryKey { get; set; }

        private PropertySnapshot(
                string name,
                object? currentValue,
                object? originalValue,
                bool isModified,
                bool isPrimaryKey,
                bool isTemporary)
        {
            Name = name;
            CurrentValue = currentValue;
            OriginalValue = originalValue;
            IsModified = isModified;
            IsPrimaryKey = isPrimaryKey;
            IsTemporary = isTemporary;
        }

        public static PropertySnapshot Create(IPropertyAdapter adapter, EntryState state)
        {
            bool isTemp = adapter.IsTemporary;

            return new PropertySnapshot(
                adapter.Name,
                isTemp ? null : adapter.CurrentValue,
                adapter.OriginalValue,
                adapter.IsModified,
                adapter.IsPrimaryKey,
                isTemp
            );
        }

        internal void UpdateValueAfterSave(object value)
        {
            CurrentValue = value;
        }
    }
}
