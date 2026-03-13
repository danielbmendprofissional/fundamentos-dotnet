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
using Valuto.AuditTrail.Abstractions.Model.DTOs;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.Logic.Cache
{
    public class MetadataCacheProxy : IMetadataAdapter
    {
        private readonly IMetadataAdapter _inner;

        private static readonly ConcurrentDictionary<object, IEnumerable<PropertyMetadata>> _propertiesCache
            = new ConcurrentDictionary<object, IEnumerable<PropertyMetadata>>();

        private static readonly ConcurrentDictionary<object, IEnumerable<ForeignKeyMetadata>> _foreignKeysCache
            = new ConcurrentDictionary<object, IEnumerable<ForeignKeyMetadata>>();

        private static readonly ConcurrentDictionary<object, IEnumerable<NavigationMetadata>> _navigationsCache
            = new ConcurrentDictionary<object, IEnumerable<NavigationMetadata>>();

        private static readonly ConcurrentDictionary<object, string> _principalEntityNameCache
            = new ConcurrentDictionary<object, string>();

        private static readonly ConcurrentDictionary<Type, ValutoAuditableAttribute?> _attributeCache
            = new ConcurrentDictionary<Type, ValutoAuditableAttribute?>();

        private static readonly ConcurrentDictionary<Type, string> _namespaceCache
            = new ConcurrentDictionary<Type, string>();

        public MetadataCacheProxy(IMetadataAdapter inner)
        {
            _inner = inner;
        }

        public IEnumerable<PropertyMetadata> GetProperties(object metadata) =>
            metadata == null ? Enumerable.Empty<PropertyMetadata>() : _propertiesCache.GetOrAdd(metadata, m => _inner.GetProperties(m));

        public IEnumerable<ForeignKeyMetadata> GetForeignKeys(object metadata) =>
            metadata == null ? Enumerable.Empty<ForeignKeyMetadata>() : _foreignKeysCache.GetOrAdd(metadata, m => _inner.GetForeignKeys(m)!);

        public IEnumerable<NavigationMetadata> GetNavigations(object metadata) =>
            metadata == null ? Enumerable.Empty<NavigationMetadata>() : _navigationsCache.GetOrAdd(metadata, m => _inner.GetNavigations(m));

        public string GetPrincipalEntityName(object metadata) =>
            metadata == null ? string.Empty : _principalEntityNameCache.GetOrAdd(metadata, m => _inner.GetPrincipalEntityName(m));

        public ValutoAuditableAttribute? GetAuditableAttribute(Type? type) =>
            type == null ? null : _attributeCache.GetOrAdd(type, t => _inner.GetAuditableAttribute(t));

        public string GetNamespace(Type? type) =>
            type == null ? AuditConstants.Default : _namespaceCache.GetOrAdd(type, t => _inner.GetNamespace(t));
    }
}
