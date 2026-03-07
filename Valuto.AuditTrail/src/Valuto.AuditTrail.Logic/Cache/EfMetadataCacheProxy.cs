using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Valuto.AuditTrail.Abstractions;
using Valuto.AuditTrail.Abstractions.Commom;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.Logic.Cache
{
    public class EfMetadataCacheProxy : IEfMetadataAdapter
    {
        private readonly IEfMetadataAdapter _inner;

        private static readonly ConcurrentDictionary<object, string> _propertyNameCache 
            = new ConcurrentDictionary<object, string>();

        private static readonly ConcurrentDictionary<object, object> _primaryKeyMetadataCache 
            = new ConcurrentDictionary<object, object>();

        private static readonly ConcurrentDictionary<object, IEnumerable> _metadataPropertiesCache 
            = new ConcurrentDictionary<object, IEnumerable>();

        private static readonly ConcurrentDictionary<object, IEnumerable> _foreignKeysCache 
            = new ConcurrentDictionary<object, IEnumerable>();

        private static readonly ConcurrentDictionary<object, string> _navNameFromFkCache 
            = new ConcurrentDictionary<object, string>();

        private static readonly ConcurrentDictionary<object, string> _firstFkPropNameCache 
            = new ConcurrentDictionary<object, string>();

        private static readonly ConcurrentDictionary<Type, ValutoAuditableAttribute> _attributeCache
            = new ConcurrentDictionary<Type, ValutoAuditableAttribute>();

        private static readonly ConcurrentDictionary<Type, string> _entityNameCache 
            = new ConcurrentDictionary<Type, string>();

        private static readonly ConcurrentDictionary<Type, string> _namespaceCache 
            = new ConcurrentDictionary<Type, string>();

        private static readonly ConcurrentDictionary<object, string> _cleanEntityNameCache 
            = new ConcurrentDictionary<object, string>();

        private static readonly ConcurrentDictionary<object, IEnumerable<object>> _collectionsMetadataCache 
            = new ConcurrentDictionary<object, IEnumerable<object>>();

        private static readonly ConcurrentDictionary<object, IEnumerable<object>> _referencesMetadataCache
            = new ConcurrentDictionary<object, IEnumerable<object>>();

        public EfMetadataCacheProxy(IEfMetadataAdapter inner)
        {
            _inner = inner;
        }

        public string? GetPropertyName(object metadata) =>
            metadata == null ? null : _propertyNameCache.GetOrAdd(metadata, m => _inner.GetPropertyName(m)!);

        public object? FindPrimaryKey(object typeMetadata) =>
            typeMetadata == null ? null : _primaryKeyMetadataCache.GetOrAdd(typeMetadata, t => _inner.FindPrimaryKey(t)!);

        public IEnumerable? GetPropertiesFromMetadata(object metadataContainer) =>
            metadataContainer == null ? null : _metadataPropertiesCache.GetOrAdd(metadataContainer, m => _inner.GetPropertiesFromMetadata(m)!);

        public IEnumerable? GetForeignKeys(object typeMetadata) =>
            typeMetadata == null ? null : _foreignKeysCache.GetOrAdd(typeMetadata, t => _inner.GetForeignKeys(t)!);

        public string? GetNavigationNameFromForeignKey(object fk) =>
            fk == null ? null : _navNameFromFkCache.GetOrAdd(fk, f => _inner.GetNavigationNameFromForeignKey(f)!);

        public string? GetFirstFkPropertyName(object fk) =>
            fk == null ? null : _firstFkPropNameCache.GetOrAdd(fk, f => _inner.GetFirstFkPropertyName(f)!);

        public ValutoAuditableAttribute? GetAuditableAttribute(Type type) =>
            type == null ? null : _attributeCache.GetOrAdd(type, t => _inner.GetAuditableAttribute(t)!);

        public string GetEntityName(Type type) =>
            type == null ? AuditConstants.Default : _entityNameCache.GetOrAdd(type, t => _inner.GetEntityName(t));

        public string GetNamespace(Type type) =>
            type == null ? AuditConstants.Default : _namespaceCache.GetOrAdd(type, t => _inner.GetNamespace(t));

        public string? GetCleanEntityName(object metadata) =>
            metadata == null ? null : _cleanEntityNameCache.GetOrAdd(metadata, m => _inner.GetCleanEntityName(m)!);

        public IEnumerable<object> GetCollectionsMetadata(object typeMetadata) =>
            typeMetadata == null ? Enumerable.Empty<object>() : _collectionsMetadataCache.GetOrAdd(typeMetadata, t => _inner.GetCollectionsMetadata(t));

        public IEnumerable<object> GetReferencesMetadata(object typeMetadata) =>
            typeMetadata == null ? Enumerable.Empty<object>() : _referencesMetadataCache.GetOrAdd(typeMetadata, t => _inner.GetReferencesMetadata(t));

        public AuditEntryState GetEntityState(IAuditTracker tracker, object entity) =>
            _inner.GetEntityState(tracker, entity);

        public object? GetCurrentValue(object navEntry) => _inner.GetCurrentValue(navEntry);

        public bool IsNavigationModified(object navEntry) => _inner.IsNavigationModified(navEntry);
    }
}
