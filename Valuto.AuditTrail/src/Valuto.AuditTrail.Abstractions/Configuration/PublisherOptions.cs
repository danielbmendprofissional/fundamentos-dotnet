using System;
using System.Collections.Generic;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Configuration
{
    /// <summary>
    /// Define o comportamento do transporte de mensagens.
    /// </summary>
    public class PublisherOptions
    {
        /// <summary>
        /// Indica se o sistema deve tentar reprocessar o envio em caso de falha no transporte.
        /// O valor padrão é true.
        /// </summary>
        public bool RetryOnFailure { get; set; } = true;
    }
}
