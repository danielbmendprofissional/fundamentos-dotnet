using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Common
{
    internal static class Guard
    {
        public static void AgainstEmptyGuid(Guid id, string paramName)
        {
            if (id == Guid.Empty)
                throw new DomainException($"{paramName} cannot be Guid.empty");
        }

        public static void AgainstNull<T>(T value, string paramName)
        {
            if (value == null)
                throw new DomainException($"{paramName} cannot be null");
        }

        public static void AgainstNullOrWhiteSpace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"{paramName} cannot be null or empty");
        }

        public static void AgainstInvalidEnum<TEnum>(object value, string paramName) where TEnum : Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
                throw new DomainException($"invalid param {paramName}");
        }

        public static void Against<TException>(bool condition, string message) where TException : Exception
        {
            if (condition) throw (TException)Activator.CreateInstance(typeof(TException), message)!;
        }
    }
}
