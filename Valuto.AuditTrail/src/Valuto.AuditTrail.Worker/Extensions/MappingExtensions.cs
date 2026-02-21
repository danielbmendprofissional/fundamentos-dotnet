using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Valuto.AuditTrail.Core
{
    public static class MappingExtensions
    {
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> builder, string columnType, string fieldName)
        {
            return builder
                .HasColumnType(columnType)
                .HasField(fieldName)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<T>(v, (JsonSerializerOptions)null)
                );
        }
    }
}
