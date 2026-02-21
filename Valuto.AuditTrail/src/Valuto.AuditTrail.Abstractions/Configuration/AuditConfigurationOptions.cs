using System;
using System.Collections.Generic;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Configuration
{
    /// <summary>
    /// Opções centrais para configuração do sistema de trilha de auditoria.
    /// </summary>
    public class AuditConfigurationOptions
    {
        /// <summary>
        /// Obtém as configurações de buffer em memória (Channel).
        /// </summary>
        public BufferOptions Buffer { get; } = new BufferOptions();

        /// <summary>
        /// Obtém as configurações de despacho e publicação de eventos.
        /// </summary>
        public PublisherOptions Publisher { get; } = new PublisherOptions();

        /// <summary>
        /// Configura o comportamento do buffer de retenção temporária dos eventos.
        /// </summary>
        /// <param name="configure">Ação para definir propriedades do buffer.</param>
        public void ConfigureBuffer(Action<BufferOptions> configure) => configure(Buffer);

        /// <summary>
        /// Configura as estratégias de publicação e envio para o transporte de mensagens.
        /// </summary>
        /// <param name="configure">Ação para definir propriedades do publicador.</param>
        public void ConfigurePublisher(Action<PublisherOptions> configure) => configure(Publisher);
    }
}
