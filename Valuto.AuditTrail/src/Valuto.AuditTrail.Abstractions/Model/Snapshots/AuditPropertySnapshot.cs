using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis.System.Diagnostics.CodeAnalysis;
using System.Text;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots
{
    /// <summary>
    /// Representa o estado capturado de uma propriedade individual para a trilha de auditoria.
    /// Implementa suporte a valores temporários (Lazy Loading) para chaves geradas pelo banco de dados.
    /// </summary>
    public sealed class AuditPropertySnapshot : IAuditProperty
    {
        /// <summary>
        /// Obtém ou define o nome técnico da propriedade no mapeamento do Entity Framework.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o valor original da propriedade antes da modificação.
        /// </summary>
        [AllowNull]
        public object? OriginalValue { get; set; }

        // OBRIGATORIEDADE: Campos internos para suporte ao padrão de carregamento tardio (Deferred Loading)
        [AllowNull]
        private Func<object>? _valueProvider;
        [AllowNull]
        private object? _staticCurrentValue;

        /// <summary>
        /// Obtém o valor atual da propriedade. 
        /// Se <see cref="IsTemporary"/> for verdadeiro, invoca o provedor de valor para capturar dados gerados pelo banco.
        /// </summary>
        [AllowNull]
        public object? CurrentValue
        {
            get => IsTemporary ? _valueProvider?.Invoke() : _staticCurrentValue;
            set => _staticCurrentValue = value;
        }

        /// <summary>
        /// Indica se o valor é temporário (ex: PKs não persistidas) e requer resolução posterior via <see cref="_valueProvider"/>.
        /// </summary>
        public bool IsTemporary { get; set; }

        /// <summary>
        /// Indica se houve alteração entre o valor original e o valor atual durante o ciclo de vida da transação.
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        /// Armazena metadados adicionais da propriedade para uso extensível por outros adaptadores.
        /// </summary>
        [AllowNull]
        public object? Metadata { get; set; }

        /// <summary>
        /// Configura um delegado para carregamento tardio do valor, marcando a propriedade como temporária.
        /// </summary>
        /// <param name="loader">Função que será executada para resolver o valor final da propriedade.</param>
        public void SetLazyLoader(Func<object> loader)
        {
            _valueProvider = loader;
            IsTemporary = true;
        }
    }
}
