using Galo.Application.Common.Base;

namespace Galo.Infrastructure.Messaging
{
    /// <summary>
    /// Mapeia "nome do evento" (que viaja na mensagem) para o Type .NET na hora de DESSERIALIZAR.
    /// Não serializamos o nome de classe completo (acoplaria produtor e consumidor ao namespace .NET).
    /// </summary>
    public sealed class IntegrationEventTypeRegistry
    {
        private readonly Dictionary<string, Type> _tipos;

        public IntegrationEventTypeRegistry()
        {
            _tipos = typeof(IntegrationEvent).Assembly.GetTypes()
                .Where(t => typeof(IntegrationEvent).IsAssignableFrom(t) && !t.IsAbstract)
                .ToDictionary(t => t.Name, t => t, StringComparer.Ordinal);
        }

        public Type? Resolver(string nome) => _tipos.GetValueOrDefault(nome);
    }
}
