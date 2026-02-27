using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions;
using Valuto.AuditTrail.Abstractions.Commom;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.EFCore.Adapters
{
    internal class EfMetadataAdapter : IEfMetadataAdapter
    {
        public string GetPropertyName(object metadata) =>
#if NETCOREAPP3_1
            metadata is IPropertyBase prop ? prop.Name : metadata?.ToString();
#else
            metadata is IReadOnlyPropertyBase prop ? prop.Name : metadata?.ToString();
#endif
        public object FindPrimaryKey(object typeMetadata) =>
#if NETCOREAPP3_1
            typeMetadata is IEntityType entity ? entity.FindPrimaryKey() : null;
#else
            typeMetadata is IReadOnlyEntityType entity ? entity.FindPrimaryKey() : null;
#endif
        public IEnumerable GetPropertiesFromMetadata(object metadataContainer)
        {
#if NETCOREAPP3_1
            if (metadataContainer is IKey key) return key.Properties;
            if (metadataContainer is IForeignKey fk) return fk.Properties;
#elif NET6_0 || NET7_0
            if (metadataContainer is IReadOnlyKey key) return key.Properties;
            if (metadataContainer is IReadOnlyForeignKey fk) return fk.Properties;
#else
            if (metadataContainer is IKey key) return key.Properties;
            if (metadataContainer is IReadOnlyForeignKey fk) return fk.Properties;
#endif
            return null;

        }

        public IEnumerable GetForeignKeys(object typeMetadata) =>
#if NETCOREAPP3_1
            typeMetadata is IEntityType entity ? entity.GetForeignKeys() : null;
#else
            typeMetadata is IReadOnlyEntityType entity ? entity.GetForeignKeys() : null;
#endif

        public string GetNavigationNameFromForeignKey(object fk) =>
#if NETCOREAPP3_1
            fk is IForeignKey foreignKey
                ? (foreignKey.DependentToPrincipal?.Name ?? foreignKey.PrincipalToDependent?.Name)
                : null;
#else
            fk is IReadOnlyForeignKey foreignKey
                ? (foreignKey.DependentToPrincipal?.Name ?? foreignKey.PrincipalToDependent?.Name)
                : null;
#endif

        public string GetFirstFkPropertyName(object fk) =>
#if NETCOREAPP3_1
            fk is IForeignKey foreignKey ? foreignKey.Properties.FirstOrDefault()?.Name : null;
#else
            fk is IReadOnlyForeignKey foreignKey ? foreignKey.Properties.FirstOrDefault()?.Name : null;
#endif

        public ValutoAuditableAttribute GetAuditableAttribute(Type type) =>
            GetActualType(type).GetCustomAttribute<ValutoAuditableAttribute>(true);

        public string GetEntityName(Type type) => GetActualType(type).Name;

        public string GetNamespace(Type type) => GetActualType(type).Namespace ?? AuditConstants.Default;

        public string GetCleanEntityName(object metadata) =>
#if NETCOREAPP3_1
            metadata is IEntityType entity ? entity.ClrType.Name : metadata?.ToString().Split('.').Last();
#else
            metadata is IReadOnlyEntityType entity ? entity.ClrType.Name : metadata?.ToString().Split('.').Last();
#endif

        public IEnumerable<object> GetCollectionsMetadata(object typeMetadata) =>
#if NETCOREAPP3_1
            typeMetadata is IEntityType entity ? entity.GetNavigations().Where(n => n.IsCollection()).Cast<object>() : Enumerable.Empty<object>();
#else
            typeMetadata is IReadOnlyEntityType entity ? entity.GetNavigations().Where(n => n.IsCollection) : Enumerable.Empty<object>();
#endif

        public IEnumerable<object> GetReferencesMetadata(object typeMetadata) =>
#if NETCOREAPP3_1
            typeMetadata is IEntityType entity ? entity.GetNavigations().Where(n => !n.IsCollection()).Cast<object>() : Enumerable.Empty<object>();
#else
            typeMetadata is IReadOnlyEntityType entity ? entity.GetNavigations().Where(n => !n.IsCollection) : Enumerable.Empty<object>();
#endif


        public AuditEntryState GetEntityState(IAuditTracker tracker, object entity) =>
            tracker.Entries().FirstOrDefault(e => e.Entity == entity)?.State ?? AuditEntryState.Unchanged;

        private Type GetActualType(Type type) =>
            (type.FullName.StartsWith(AuditConstants.Proxies) || type.FullName.StartsWith(AuditConstants.EFProxies))
                ? type.BaseType : type;

        public object GetCurrentValue(object navEntry)
        {
            if (navEntry is Microsoft.EntityFrameworkCore.ChangeTracking.NavigationEntry entry)
                return entry.CurrentValue;

            return null;
        }

        public bool IsNavigationModified(object navEntry)
        {
            return navEntry is Microsoft.EntityFrameworkCore.ChangeTracking.NavigationEntry entry
                   && entry.IsModified;
        }
    }
}
