using System;
using System.Collections.Generic;
using Valuto.AuditTrail.Abstractions.Commom;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Snapshots;

namespace Valuto.AuditTrail.Abstractions.Model.Helpers
{
    /// <summary>
    /// Objeto de contexto temporário utilizado para manter o estado da auditoria durante o ciclo de vida do SaveChanges.
    /// </summary>
    public sealed class AuditProcessingContextHelper
    {
        /// <summary>
        /// Identificador único da transação atual.
        /// </summary>
        public Guid TransactionId { get; }

        /// <summary>
        /// Descritor da etapa atual do processo (utilizado para telemetria e logs de erro).
        /// </summary>
        public string Step { get; set; }

        /// <summary>
        /// Referência para a entrada que está sendo processada no momento.
        /// </summary>
        /// <remarks>
        /// Utiliza <see cref="EntitySnapshot"/> para permitir a captura de IDs via Reflection em caso de erro.
        /// </remarks>
        public EntitySnapshot CurrentEntry { get; set; }

        /// <summary>
        /// Inicializa um novo contexto de processamento com um ID de transação único.
        /// </summary>
        public AuditProcessingContextHelper()
        {
            TransactionId = Guid.NewGuid();
            Step = AuditConstants.Inicializando;
        }

        public void Start(EntitySnapshot entity)
        {
            CurrentEntry = entity;
            Step = AuditConstants.Criacao;
        }

        /// <summary>
        /// Atualiza o rastro de processamento para auxiliar no rastreamento de erros (SafeAction).
        /// </summary>
        /// <param name="entry">Entrada atual.</param>
        /// <param name="step">Etapa atual.</param>
        public void Track(string step)
        {
            Step = step + CurrentEntry.Name;
        }
    }
}
