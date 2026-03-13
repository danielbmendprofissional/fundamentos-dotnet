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
using Valuto.AuditTrail.Abstractions.Model.DTOs;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.EFCore.Adapters
{
    internal class MetadataAdapter : IMetadataAdapter
    {
        public IEnumerable<PropertyMetadata> GetProperties(object metadata)
        {
#if NETCOREAPP3_1
            var entity = metadata as IEntityType;
#else
            var entity = metadata as IReadOnlyEntityType;
#endif
            if (entity == null)
            {
                return Enumerable.Empty<PropertyMetadata>();
            }

            var pkProperties = entity.FindPrimaryKey()?.Properties
                .Select(p => p.Name)
                .ToList() ?? new List<string>();

            return entity.GetProperties().Select(p =>
                new PropertyMetadata(
                    p.Name,
                    pkProperties.Contains(p.Name)
                )).ToList();
        }

        public IEnumerable<ForeignKeyMetadata>? GetForeignKeys(object metadata)
        {
#if NETCOREAPP3_1
            var entity = metadata as IEntityType;
#else
            var entity = metadata as IReadOnlyEntityType;
#endif

            if (entity == null)
                return Enumerable.Empty<ForeignKeyMetadata>();

            return entity.GetForeignKeys()
                .Select(fk =>
                {
                    var navigationName = GetNavigationNameFromForeignKey(fk);
                    var propertyName = GetFirstFkPropertyName(fk) ?? AuditConstants.Unknown;

                    return new ForeignKeyMetadata(
                        name: navigationName ?? propertyName,
                        propertyName: propertyName
                    );
                }).ToList();
        }

        public IEnumerable<NavigationMetadata> GetNavigations(object metadata)
        {
#if NETCOREAPP3_1
            if (metadata is IEntityType entity)
            {
                return entity.GetNavigations()
                    .Select(n => {
                        var targetEntityType = n.IsDependentToPrincipal()
                            ? n.ForeignKey.PrincipalEntityType
                            : n.ForeignKey.DeclaringEntityType;

                        var pks = targetEntityType.FindPrimaryKey()?.Properties
                            .Select(p => p.Name).ToArray() ?? Array.Empty<string>();

                        return new NavigationMetadata(n.Name, n.IsCollection(), pks);
                    })
                    .ToList();
            }
#else
            if (metadata is IReadOnlyEntityType entity)
            {
                return entity.GetNavigations()
                    .Select(n => new NavigationMetadata(
                        n.Name, 
                        n.IsCollection,
                        n.TargetEntityType.FindPrimaryKey()?.Properties.Select(p => p.Name).ToArray() ?? Array.Empty<string>()
                    ))
                    .ToList();
            }
#endif

            return Enumerable.Empty<NavigationMetadata>();
        }

        public string GetPrincipalEntityName(object metadata)
        {
#if NETCOREAPP3_1
            var property = metadata as IProperty;
#else
        var property = metadata as IReadOnlyProperty;
#endif
            if (property == null) return string.Empty;

            var fk = property.GetContainingForeignKeys().FirstOrDefault();
            return fk?.PrincipalEntityType.Name ?? string.Empty;
        }

        public ValutoAuditableAttribute? GetAuditableAttribute(Type? type) =>
            GetActualType(type)?.GetCustomAttribute<ValutoAuditableAttribute>(true);

        public string GetNamespace(Type? type) => GetActualType(type)?.Namespace ?? AuditConstants.Default;

        private string? GetNavigationNameFromForeignKey(object fk) =>
#if NETCOREAPP3_1
            fk is IForeignKey foreignKey
                ? (foreignKey.DependentToPrincipal?.Name ?? foreignKey.PrincipalToDependent?.Name)
                : null;
#else
            fk is IReadOnlyForeignKey foreignKey
                ? (foreignKey.DependentToPrincipal?.Name ?? foreignKey.PrincipalToDependent?.Name)
                : null;
#endif

        private string? GetFirstFkPropertyName(object fk) =>
#if NETCOREAPP3_1
            fk is IForeignKey foreignKey ? foreignKey.Properties.FirstOrDefault()?.Name : null;
#else
            fk is IReadOnlyForeignKey foreignKey ? foreignKey.Properties.FirstOrDefault()?.Name : null;
#endif

        private Type? GetActualType(Type? type) =>
            (type != null && type?.FullName != null && (type.FullName.StartsWith(AuditConstants.Proxies) || type.FullName.StartsWith(AuditConstants.EFProxies)))
                ? type.BaseType : type;
    }
}
