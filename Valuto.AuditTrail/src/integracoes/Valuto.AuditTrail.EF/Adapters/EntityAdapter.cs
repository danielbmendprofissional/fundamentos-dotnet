using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria.Adapters;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;
using Valuto.AuditTrail.EF.Adapters;

namespace Valuto.AuditTrail.EF
{
    internal class EntityAdapter : IEntityAdapter
    {
        private readonly DbContext _context;
        private readonly DbEntityEntry _entry;
        private readonly IMetadataAdapter _metadataAdapter;
        private IEnumerable<IPropertyAdapter> _properties;
        private IEnumerable<IForeignKeyAdapter> _foreignKeys;
        private IEnumerable<INavigationAdapter> _navigations;
        private IEnumerable<IReferenceAdapter> _references;

        public EntityAdapter(DbContext context, DbEntityEntry entry, IMetadataAdapter metadataAdapter)
        {
            _context = context;
            _entry = entry;
            _metadataAdapter = metadataAdapter;
        }

        public Type Type => ObjectContext.GetObjectType(_entry.Entity.GetType());

        public string Name => ObjectContext.GetObjectType(_entry.Entity.GetType()).Name;
        public string Namespace => ObjectContext.GetObjectType(_entry.Entity.GetType()).Namespace;

        public EntryState State => MapState(_entry.State);

        public IEnumerable<IPropertyAdapter> Properties
        {
            get
            {
                if (_properties == null)
                {
                    var objectContext = ((IObjectContextAdapter)_context).ObjectContext;
                    var workspace = objectContext.MetadataWorkspace;

                    var type = ObjectContext.GetObjectType(_entry.Entity.GetType());

                    var conceptualEntity = workspace.GetItems<EntityType>(DataSpace.CSpace)
                        .FirstOrDefault(e => e.Name == type.Name);

                    if (conceptualEntity != null)
                    {
                        var propertiesMetadata = _metadataAdapter.GetProperties(conceptualEntity);

                        _properties = propertiesMetadata.Select(p =>
                            new PropertyAdapter(
                                _entry,
                                p.Name,
                                p.IsPrimaryKey
                            )).ToList();
                    }
                    else
                    {
                        _properties = Enumerable.Empty<IPropertyAdapter>();
                    }
                }
                return _properties;
            }
        }

        public IEnumerable<IForeignKeyAdapter> ForeignKeys
        {
            get
            {
                if (_foreignKeys == null)
                {
                    var list = new List<IForeignKeyAdapter>();
                    var objectContext = ((IObjectContextAdapter)_context).ObjectContext;

                    var entityType = ObjectContext.GetObjectType(_entry.Entity.GetType());
                    var workspace = objectContext.MetadataWorkspace;

                    var conceptualEntity = workspace.GetItems<EntityType>(DataSpace.CSpace)
                        .FirstOrDefault(e => e.Name == entityType.Name);

                    if (conceptualEntity != null)
                    {
                        foreach (var navProperty in conceptualEntity.NavigationProperties)
                        {
                            var association = navProperty.RelationshipType as AssociationType;
                            if (association?.ReferentialConstraints.Any() == true)
                            {
                                var constraint = association.ReferentialConstraints.First();

                                if (constraint.ToRole.Name == navProperty.FromEndMember.Name)
                                {
                                    foreach (var prop in constraint.ToProperties)
                                    {
                                        var dbProperty = _entry.Property(prop.Name);

                                        bool isTemporary = _entry.State == EntityState.Added && IsDefaultValue(dbProperty.CurrentValue);

                                        list.Add(new ForeignKeyAdapter(
                                            dbProperty,
                                            prop.Name,
                                            _metadataAdapter,
                                            navProperty,
                                            isTemporary
                                        ));
                                    }
                                }
                            }
                        }
                    }
                    _foreignKeys = list;
                }
                return _foreignKeys;
            }
        }

        public IEnumerable<INavigationAdapter> Navigations
        {
            get
            {
                if (_navigations == null)
                {
                    var objectContext = ((IObjectContextAdapter)_context).ObjectContext;
                    var workspace = objectContext.MetadataWorkspace;

                    var type = ObjectContext.GetObjectType(_entry.Entity.GetType());

                    var conceptualEntity = workspace.GetItems<EntityType>(DataSpace.CSpace)
                        .FirstOrDefault(e => e.Name == type.Name);

                    if (conceptualEntity != null)
                    {

                        var navigationsMetadata = _metadataAdapter.GetNavigations(conceptualEntity)
                            .Where(n => n.IsCollection);

                        _navigations = navigationsMetadata.Select(n =>
                        {
                            var collectionEntry = _entry.Collection(n.Name);

                            return new NavigationAdapter(collectionEntry, true);
                        }).ToList();
                    }
                    else
                    {
                        _navigations = Enumerable.Empty<INavigationAdapter>();
                    }
                }
                return _navigations;
            }
        }

        public IEnumerable<IReferenceAdapter> References
        {
            get
            {
                if (_references == null)
                {
                    var list = new List<IReferenceAdapter>();
                    var objectContext = ((IObjectContextAdapter)_context).ObjectContext;
                    var workspace = objectContext.MetadataWorkspace;

                    var entityType = workspace.GetItems<EntityType>(DataSpace.CSpace)
                        .FirstOrDefault(e => e.Name == this.Name);

                    if (entityType != null)
                    {
                        var navigations = _metadataAdapter.GetNavigations(entityType)
                            .Where(n => !n.IsCollection);

                        var allForeignKeys = _metadataAdapter.GetForeignKeys(entityType);

                        foreach (var nav in navigations)
                        {
                            var dependentProps = allForeignKeys
                                .Where(fk => fk.Name == nav.Name)
                                .Select(fk => fk.PropertyName)
                                .ToList();

                            list.Add(new ReferenceAdapter(
                                _entry,
                                nav.Name,
                                dependentProps
                            ));
                        }
                    }
                    _references = list;
                }
                return _references;
            }
        }

        private EntryState MapState(EntityState state)
        {
            switch (state)
            {
                case EntityState.Added:
                    return EntryState.Added;
                case EntityState.Modified:
                    return EntryState.Modified;
                case EntityState.Deleted:
                    return EntryState.Deleted;
                default:
                    return EntryState.Unchanged;
            }
        }

        private bool IsDefaultValue(object value)
        {
            if (value == null) return true;
            var type = value.GetType();
            return type.IsValueType ? value.Equals(Activator.CreateInstance(type)) : false;
        }
    }
}
