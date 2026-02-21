using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions;
using Valuto.AuditTrail.Abstractions.Commom;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.EF.Adapters
{
    internal class EfMetadataAdapter : IEfMetadataAdapter
    {
        public string GetPropertyName(object metadata)
        {
            if (metadata == null) return null;

            if (metadata is EdmMember member)
                return member.Name;

            if (metadata is DbPropertyEntry entry)
                return entry.Name;

            return metadata.ToString();
        }

        public object FindPrimaryKey(object typeMetadata)
        {
            if (typeMetadata is EntityType entityType)
                return entityType.KeyMembers;

            return null;
        }

        public IEnumerable GetPropertiesFromMetadata(object metadataContainer)
        {
            if (metadataContainer is IEnumerable members) return members;
            return null;
        }

        public IEnumerable GetForeignKeys(object typeMetadata)
        {
            if (typeMetadata is EntityType entityType)
            {
                return entityType.NavigationProperties
                    .Where(n => n.GetDependentProperties().Any());
            }
            return Enumerable.Empty<object>();
        }

        public string GetNavigationNameFromForeignKey(object fk)
        {
            if (fk is NavigationProperty nav) return nav.Name;
            return null;
        }

        public string GetFirstFkPropertyName(object fk)
        {
            if (fk is NavigationProperty nav)
            {
                return nav.GetDependentProperties().FirstOrDefault()?.Name;
            }
            return null;
        }

        public ValutoAuditableAttribute GetAuditableAttribute(Type type)
        {
            return GetActualType(type).GetCustomAttribute<ValutoAuditableAttribute>(true);
        }

        public string GetEntityName(Type type) => GetActualType(type).Name;

        public string GetNamespace(Type type) => GetActualType(type).Namespace ?? AuditConstants.Default;

        public string GetCleanEntityName(object metadata)
        {
            if (metadata is EntityType entityType) return entityType.Name;
            return metadata?.ToString().Split('.').Last();
        }

        public IEnumerable<object> GetCollectionsMetadata(object typeMetadata)
        {
            if (typeMetadata is EntityType entityType)
            {
                return entityType.NavigationProperties
                    .Where(n => n.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
                    .Cast<object>();
            }
            return Enumerable.Empty<object>();
        }

        public IEnumerable<object> GetReferencesMetadata(object typeMetadata)
        {
            if (typeMetadata is EntityType entityType)
            {
                return entityType.NavigationProperties
                    .Where(n => n.ToEndMember.RelationshipMultiplicity != RelationshipMultiplicity.Many)
                    .Cast<object>();
            }
            return Enumerable.Empty<object>();
        }

        public AuditEntryState GetEntityState(IAuditTracker tracker, object entity)
        {
            var entry = tracker.Entries().FirstOrDefault(e => e.Entity == entity);
            return entry?.State ?? AuditEntryState.Unchanged;
        }

        private Type GetActualType(Type type)
        {
            if (type.FullName.StartsWith(AuditConstants.Proxies) ||
                type.Namespace == "System.Data.Entity.DynamicProxies")
            {
                return type.BaseType;
            }
            return type;
        }

        public object GetCurrentValue(object navEntry)
        {
            if (navEntry is System.Data.Entity.Infrastructure.DbMemberEntry entry)
                return entry.CurrentValue;

            if (navEntry is System.Data.Entity.Infrastructure.DbPropertyEntry prop)
                return prop.CurrentValue;

            return null;
        }

        public bool IsNavigationModified(object navEntry)
        {
            if (navEntry is System.Data.Entity.Infrastructure.DbMemberEntry entry)
            {
                return entry.EntityEntry.State == System.Data.Entity.EntityState.Modified;
            }
            return false;
        }
    }
}
