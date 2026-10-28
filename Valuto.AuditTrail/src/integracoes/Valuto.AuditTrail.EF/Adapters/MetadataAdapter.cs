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
using Valuto.AuditTrail.Abstractions.Model.DTOs;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.EF.Adapters
{
    internal class MetadataAdapter : IMetadataAdapter
    {
        public IEnumerable<PropertyMetadata> GetProperties(object metadata)
        {
            if (metadata is EntityType entity)
            {
                var pkProperties = entity.KeyMembers
                    .Select(p => p.Name)
                    .ToList();

                return entity.Properties.Select(p =>
                    new PropertyMetadata(
                        p.Name,
                        pkProperties.Contains(p.Name)
                    )).ToList();
            }

            return Enumerable.Empty<PropertyMetadata>();
        }

        public IEnumerable<ForeignKeyMetadata> GetForeignKeys(object metadata)
        {
            if (metadata is EntityType entity)
            {
                var list = new List<ForeignKeyMetadata>();
                foreach (var nav in entity.NavigationProperties)
                {
                    var association = nav.RelationshipType as AssociationType;
                    var constraint = association?.ReferentialConstraints.FirstOrDefault();

                    if (constraint != null && constraint.ToRole.Name == nav.FromEndMember.Name)
                    {
                        foreach (var prop in constraint.ToProperties)
                        {
                            list.Add(new ForeignKeyMetadata(
                                name: nav.Name,
                                propertyName: prop.Name
                            ));
                        }
                    }
                }
                return list.ToList();
            }
            return Enumerable.Empty<ForeignKeyMetadata>();
        }

        public IEnumerable<NavigationMetadata> GetNavigations(object metadata)
        {
            if (metadata is EntityType entity)
            {
                return entity.NavigationProperties
                    .Select(n =>
                    {
                        var targetEntityType = n.ToEndMember.GetEntityType();

                        var targetPkNames = targetEntityType?.KeyMembers
                            .Select(k => k.Name)
                            .ToArray() ?? Array.Empty<string>();

                        return new NavigationMetadata(
                            n.Name,
                            n.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many,
                            targetPkNames
                        );
                    }).ToList();
            }
            return Enumerable.Empty<NavigationMetadata>();
        }

        public string GetPrincipalEntityName(object metadata)
        {
            if (metadata is NavigationProperty nav)
            {
                return nav.ToEndMember.GetEntityType().Name;
            }
            return string.Empty;
        }

        public ValutoAuditableAttribute GetAuditableAttribute(Type type)
        {
            return GetActualType(type).GetCustomAttribute<ValutoAuditableAttribute>(true);
        }

        public string GetNamespace(Type type) => GetActualType(type).Namespace ?? AuditConstants.Default;

        private Type GetActualType(Type type)
        {
            if (type.FullName.StartsWith(AuditConstants.Proxies) ||
                type.Namespace == "System.Data.Entity.DynamicProxies")
            {
                return type.BaseType;
            }
            return type;
        }
    }
}
