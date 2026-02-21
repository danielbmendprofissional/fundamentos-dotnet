using System;
using System.Collections.Generic;
using Valuto.AuditTrail.Abstractions.Commom;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;

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
        /// Utiliza <see cref="IAuditEntry"/> para permitir a captura de IDs via Reflection em caso de erro.
        /// </remarks>
        public IAuditEntry CurrentEntry { get; set; }

        /// <summary>
        /// Mapeia instâncias de objetos reais para suas respectivas abstrações de entrada.
        /// </summary>
        public Dictionary<object, IAuditEntry> EntriesMap { get; set; }

        /// <summary>
        /// Lista de auxiliares que contêm o par (Entrada, Trilha) para processamento pós-persistência.
        /// </summary>
        public List<AuditHelper> Helpers { get; }

        /// <summary>
        /// Inicializa um novo contexto de processamento com um ID de transação único.
        /// </summary>
        public AuditProcessingContextHelper()
        {
            TransactionId = Guid.NewGuid();
            Helpers = new List<AuditHelper>();
            EntriesMap = new Dictionary<object, IAuditEntry>();
            Step = AuditConstants.PreSaveInicializando;
        }

        /// <summary>
        /// Atualiza o rastro de processamento para auxiliar no rastreamento de erros (SafeAction).
        /// </summary>
        /// <param name="entry">Entrada atual.</param>
        /// <param name="step">Etapa atual.</param>
        public void Track(IAuditEntry entry, string step)
        {
            CurrentEntry = entry;
            Step = step;
        }
    }
}
