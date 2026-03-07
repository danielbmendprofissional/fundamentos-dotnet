using System;
using Valuto.AuditTrail.Abstractions.Model.Enums;
using Valuto.AuditTrail.Abstractions.Configuration;

namespace Valuto.AuditTrail.Abstractions.Configuration
{
    /// <summary>
    /// Opções centrais para configuração do sistema de trilha de auditoria.
    /// </summary>
    public class AuditConfigurationOptions
    {
        /// <summary>
        /// Obtém o enviroment da aplicação
        /// </summary>
        public Enviroments SelectedEnviroment { get; private set; }

        /// <summary>
        /// Obtém as configurações de buffer em memória (Channel).
        /// </summary>
        public BufferOptions Buffer { get; } = new BufferOptions();

        /// <summary>
        /// Obtém as configurações de despacho e publicação de eventos.
        /// </summary>
        public PublisherOptions Publisher { get; } = new PublisherOptions();

        /// <summary>
        /// Configura o enviroment da aplicação para utilização do rabbit.
        /// </summary>
        public void ConfigureEnviroment(Enviroments enviroment)
        {
            SelectedEnviroment = enviroment;
        }

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
