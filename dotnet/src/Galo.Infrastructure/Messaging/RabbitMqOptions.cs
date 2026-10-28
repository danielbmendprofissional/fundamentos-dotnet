namespace Galo.Infrastructure.Messaging
{
    /// <summary>
    /// Seção "RabbitMQ" da configuração. Host/Port/VirtualHost/UserName/Password vêm dos USER-SECRETS (nunca de arquivo
    /// versionado). Os demais campos têm valor padrão e podem ser sobrescritos no appsettings.
    /// </summary>
    public sealed class RabbitMqOptions
    {
        public const string Secao = "RabbitMQ";

        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string VirtualHost { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        /// <summary>Exchange TOPIC onde todos os eventos de integração são publicados (routing key = nome do evento).</summary>
        public string Exchange { get; set; } = "galo.eventos";

        /// <summary>Fila durável deste serviço. Ligada ao exchange com "#" (recebe tudo).</summary>
        public string Queue { get; set; } = "galo.api.eventos";

        /// <summary>Quantas mensagens o consumidor processa em paralelo (prefetch/QoS).</summary>
        public ushort Prefetch { get; set; } = 8;

        /// <summary>Permite desligar a mensageria (ex.: testes sem broker). Publicar vira apenas log.</summary>
        public bool Habilitado { get; set; } = true;
    }
}
