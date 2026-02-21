using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Valuto.AuditTrail.Abstractions;
using Valuto.AuditTrail.Abstractions.Commom;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.EFCore3.Adapters
{
    internal class EfMetadataAdapter : IEfMetadataAdapter
    {
        public string GetPropertyName(object metadata) =>
            metadata is IPropertyBase prop ? prop.Name : metadata?.ToString();

        public object FindPrimaryKey(object typeMetadata) =>
            typeMetadata is IEntityType entity ? entity.FindPrimaryKey() : null;

        public IEnumerable GetPropertiesFromMetadata(object metadataContainer)
        {
            if (metadataContainer is IKey key) return key.Properties;
            if (metadataContainer is IForeignKey fk) return fk.Properties;
            return null;
        }

        public IEnumerable GetForeignKeys(object typeMetadata) =>
            typeMetadata is IEntityType entity ? entity.GetForeignKeys() : null;

        public string GetNavigationNameFromForeignKey(object fk) =>
            fk is IForeignKey foreignKey
                ? (foreignKey.DependentToPrincipal?.Name ?? foreignKey.PrincipalToDependent?.Name)
                : null;

        public string GetFirstFkPropertyName(object fk) =>
            fk is IForeignKey foreignKey ? foreignKey.Properties.FirstOrDefault()?.Name : null;

        public ValutoAuditableAttribute GetAuditableAttribute(Type type) =>
            GetActualType(type).GetCustomAttribute<ValutoAuditableAttribute>(true);

        public string GetEntityName(Type type) => GetActualType(type).Name;

        public string GetNamespace(Type type) => GetActualType(type).Namespace ?? AuditConstants.Default;

        public string GetCleanEntityName(object metadata) =>
            metadata is IEntityType entity ? entity.ClrType.Name : metadata?.ToString().Split('.').Last();

        public IEnumerable<object> GetCollectionsMetadata(object typeMetadata) =>
            typeMetadata is IEntityType entity ? entity.GetNavigations().Where(n => n.IsCollection()).Cast<object>() : Enumerable.Empty<object>();

        public IEnumerable<object> GetReferencesMetadata(object typeMetadata) =>
            typeMetadata is IEntityType entity ? entity.GetNavigations().Where(n => !n.IsCollection()).Cast<object>() : Enumerable.Empty<object>();

        public AuditEntryState GetEntityState(IAuditTracker tracker, object entity) =>
            tracker.Entries().FirstOrDefault(e => e.Entity == entity)?.State ?? AuditEntryState.Unchanged;

        private Type GetActualType(Type type) =>
            (type.FullName.StartsWith(AuditConstants.Proxies) || type.FullName.StartsWith(AuditConstants.EFProxies))
                ? type.BaseType : type;

        public object GetCurrentValue(object navEntry)
        {
            if (navEntry is Microsoft.EntityFrameworkCore.ChangeTracking.NavigationEntry entry)
                return entry.CurrentValue;

            if (navEntry is Microsoft.EntityFrameworkCore.ChangeTracking.PropertyEntry prop)
                return prop.CurrentValue;

            return null;
        }

        public bool IsNavigationModified(object navEntry)
        {

            if (navEntry is Microsoft.EntityFrameworkCore.ChangeTracking.NavigationEntry entry)
                return entry.IsModified;

            if (navEntry is Microsoft.EntityFrameworkCore.ChangeTracking.PropertyEntry prop)
                return prop.IsModified;

            return false;
        }
    }
}
