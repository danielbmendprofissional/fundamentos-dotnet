namespace Galo.Infrastructure.Caching
{
    /// <summary>Seção "Redis" (KeyPrefix vem dos user-secrets: "estudos:").</summary>
    public sealed class RedisOptions
    {
        public const string Secao = "Redis";

        /// <summary>
        /// Prefixo OBRIGATÓRIO de toda chave e de todo canal pub/sub. O usuário do Redis só enxerga "estudos:*"
        /// (ACL); qualquer outra chave devolve NOPERM.
        /// </summary>
        public string KeyPrefix { get; set; } = "estudos:";

        /// <summary>L1 (memória) nunca vive mais que isso, mesmo que o TTL do item seja maior: limita a defasagem
        /// caso uma mensagem de invalidação do pub/sub se perca.</summary>
        public TimeSpan TtlMaximoL1 { get; set; } = TimeSpan.FromSeconds(30);
    }
}
