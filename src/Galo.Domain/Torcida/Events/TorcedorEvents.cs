using Galo.Domain.Common.Base;
using Galo.Domain.Torcida.Enums;

namespace Galo.Domain.Torcida.Events
{
    public sealed record TorcedorCadastradoEvent(Guid TorcedorId, string Nome, string Email) : DomainEvent;

    public sealed record PlanoSocioAlteradoEvent(Guid TorcedorId, PlanoSocio PlanoAnterior, PlanoSocio PlanoNovo) : DomainEvent;

    public sealed record EnderecoPrincipalAlteradoEvent(Guid TorcedorId, Guid EnderecoId) : DomainEvent;

    public sealed record TorcedorBloqueadoEvent(Guid TorcedorId, string Motivo) : DomainEvent;
}
