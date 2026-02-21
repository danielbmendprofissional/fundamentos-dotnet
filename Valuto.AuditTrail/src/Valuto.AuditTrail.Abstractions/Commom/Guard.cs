using System;

namespace Valuto.AuditTrail.Abstractions.Commom
{
    /// <summary>
    /// Validações de domínio
    /// </summary>
    internal static class Guard
    {

        public static void AgainstEmptyGuid(Guid id, string paramName)
        {
            if (id == Guid.Empty)
                throw new DomainException($"{paramName} precisá conter valor");
        }

        public static void AgainstNullOrWhiteSpace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"{paramName} precisá conter valor");
        }

        public static void AgainstInvalidEnum<TEnum>(object value, string paramName) where TEnum : Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
                throw new DomainException($"parâmetro inválido {paramName}");
        }
    }
}
