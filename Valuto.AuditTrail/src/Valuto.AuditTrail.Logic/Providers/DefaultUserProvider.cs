using Valuto.AuditTrail.Abstractions.Commom;
using Valuto.AuditTrail.Abstractions.Contracts;

namespace Valuto.AuditTrail.Logic.Providers
{
    internal class DefaultUserProvider : IUserProvider
    {
        public string GetCurrentUser() => AuditConstants.NaoInformado;
    }
}
