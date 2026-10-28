using System;
using System.Collections;
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

        private Type _actualType;

        public EntityAdapter(DbContext context, DbEntityEntry entry, IMetadataAdapter metadataAdapter)
        {
            _context = context;
            _entry = entry;
            _metadataAdapter = metadataAdapter;
        }

        public Type Type => _actualType ?? (_actualType = ObjectContext.GetObjectType(_entry.Entity.GetType()));

        public string Name => Type.Name;
        public string Namespace => Type.Namespace;

        public EntryState State => MapState(_entry.State);

        public IEnumerable<IPropertyAdapter> Properties
        {
            get
            {
                if (_properties == null)
                {
                    var objectContext = ((IObjectContextAdapter)_context).ObjectContext;
                    var workspace = objectContext.MetadataWorkspace;

                    var conceptualEntity = workspace.GetItems<EntityType>(DataSpace.CSpace)
                        .FirstOrDefault(e => e.Name == Type.Name);

                    if (conceptualEntity != null)
                    {
                        var propertiesMetadata = _metadataAdapter.GetProperties(conceptualEntity);

                        _properties = propertiesMetadata.Select(p =>
                            new PropertyAdapter(
                                _entry,
                                p.Name,
                                p.IsPrimaryKey));
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
                    var entityType = ObjectContext.GetObjectType(this.Type);

                    var conceptualEntity = GetConceptualEntity(
                        ((IObjectContextAdapter)_context).ObjectContext.MetadataWorkspace,
                        entityType);

                    if (conceptualEntity != null)
                    {
                        _foreignKeys = _metadataAdapter.GetForeignKeys(conceptualEntity)
                            .Select(meta =>
                            {
                                var dbProperty = _entry.Property(meta.PropertyName);

                                bool isTemporary = _entry.State == EntityState.Added &&
                                                 IsDefaultValue(dbProperty.CurrentValue);

                                return new ForeignKeyAdapter(
                                    dbProperty,
                                    meta.PropertyName,
                                    isTemporary
                                );
                            });
                    }
                    else
                    {
                        _foreignKeys = Enumerable.Empty<IForeignKeyAdapter>();
                    }
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
                    var entityType = ObjectContext.GetObjectType(this.Type);

                    var conceptualEntity = GetConceptualEntity(
                        ((IObjectContextAdapter)_context).ObjectContext.MetadataWorkspace,
                        entityType);

                    if (conceptualEntity != null)
                    {

                        var navigationsMetadata = _metadataAdapter.GetNavigations(conceptualEntity)
                            .Where(n => n.IsCollection);

                        _navigations = navigationsMetadata
                            .Select(n =>
                            {
                                var collectionEntry = _entry.Collection(n.Name);
                                bool hasDataInMemory = collectionEntry.CurrentValue is IEnumerable e && e.Cast<object>().Any();

                                if (collectionEntry.IsLoaded || hasDataInMemory)
                                {
                                    return new NavigationAdapter(collectionEntry, n.PrimaryKeys);
                                }

                                return null;
                            })
                            .Where(adapter => adapter != null)
                            .ToList();
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

                    var entityType = ObjectContext.GetObjectType(this.Type);

                    var conceptualEntity = GetConceptualEntity(
                        ((IObjectContextAdapter)_context).ObjectContext.MetadataWorkspace,
                        entityType);

                    if (conceptualEntity != null)
                    {
                        var navigations = _metadataAdapter.GetNavigations(conceptualEntity).Where(n => !n.IsCollection);

                        var allForeignKeys = _metadataAdapter.GetForeignKeys(conceptualEntity);

                        var fkLookup = allForeignKeys.ToLookup(fk => fk.Name, fk => fk.PropertyName);

                        foreach (var nav in navigations)
                        {
                            var dependentProps = fkLookup[nav.Name].ToList();

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

        private EntityType GetConceptualEntity(MetadataWorkspace workspace, Type type)
        {
            if (workspace.TryGetItem<EntityType>(type.FullName, DataSpace.CSpace, out var entityByFullName))
            {
                return entityByFullName;
            }
            if (workspace.TryGetItem<EntityType>(type.Name, DataSpace.CSpace, out var entityByName))
            {
                return entityByName;
            }

            return null;
        }
    }
}
