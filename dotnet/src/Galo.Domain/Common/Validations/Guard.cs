using Galo.Domain.Common.Exceptions;

namespace Galo.Domain.Common.Validations
{
    /// <summary>Guard clauses: falhar cedo, com mensagem de negócio, antes de alterar estado.</summary>
    internal static class Guard
    {
        public static void AgainstEmptyGuid(Guid id, string paramName)
        {
            if (id == Guid.Empty)
                throw new DomainException($"{paramName} não pode ser Guid.Empty");
        }

        public static void AgainstNull<T>(T? value, string paramName)
        {
            if (value is null)
                throw new DomainException($"{paramName} não pode ser nulo");
        }

        public static void AgainstNullOrWhiteSpace(string? value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"{paramName} não pode ser nulo ou vazio");
        }

        public static void AgainstInvalidEnum<TEnum>(TEnum value, string paramName) where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(value))
                throw new DomainException($"{paramName} inválido");
        }

        public static void Against<TException>(bool condition, string message) where TException : Exception
        {
            if (condition) throw (TException)Activator.CreateInstance(typeof(TException), message)!;
        }
    }
}
