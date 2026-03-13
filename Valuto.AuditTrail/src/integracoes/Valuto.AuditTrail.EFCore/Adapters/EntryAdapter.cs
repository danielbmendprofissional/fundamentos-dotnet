using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria.Adapters;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.EFCore.Adapters
{
    internal class EntityAdapter : IEntityAdapter
    {
        private readonly EntityEntry _entry;
        private readonly IMetadataAdapter _metadataAdapter;
        private IEnumerable<IPropertyAdapter>? _properties;
        private IEnumerable<IForeignKeyAdapter>? _foreignKeys;
        private IEnumerable<INavigationAdapter>? _navigations;
        private IEnumerable<IReferenceAdapter>? _references;

        public EntityAdapter(EntityEntry entry, IMetadataAdapter metadataAdapter)
        {
            _entry = entry;
            _metadataAdapter = metadataAdapter;
        }

        public Type? Type => _entry.Metadata.ClrType;

        public string? Name => Type?.Name;

        public string? Namespace => Type?.Namespace;

        public EntryState State => MapState(_entry.State);

        public IEnumerable<IPropertyAdapter> Properties
        {
            get
            {
                if (_properties == null)
                {
                    var metadatas = _metadataAdapter.GetProperties(_entry.Metadata);

                    if (metadatas == null)
                    {
                        _properties = Enumerable.Empty<IPropertyAdapter>();
                    }
                    else
                    {
                        _properties = metadatas
                            .Select(x => new PropertyAdapter(_entry.Property(x.Name), x.IsPrimaryKey))
                            .Cast<IPropertyAdapter>()
                            .ToList();
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
                    var metadatas = _metadataAdapter.GetForeignKeys(_entry.Metadata);

                    if (metadatas == null)
                    {
                        _foreignKeys = Enumerable.Empty<IForeignKeyAdapter>();
                    }
                    else
                    {
                        _foreignKeys = metadatas
                            .Select(x => new ForeignKeyAdapter(_entry.Property(x.PropertyName), x.Name, _metadataAdapter))
                            .ToList();
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
                    var metadatas = _metadataAdapter.GetNavigations(_entry.Metadata);

                    if (metadatas == null)
                    {
                        _navigations = Enumerable.Empty<INavigationAdapter>();
                    }
                    else
                    {
                        _navigations = metadatas
                            .Where(x => x.IsCollection)
                            .Select(x => new NavigationAdapter(_entry.Navigation(x.Name), x.PrimaryKeys))
                            .ToList();
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
                    var metadatas = _metadataAdapter.GetNavigations(_entry.Metadata);

                    if (metadatas == null)
                    {
                        _references = Enumerable.Empty<IReferenceAdapter>();
                    }
                    else
                    {
                        _references = metadatas
                            .Where(x => !x.IsCollection)
                            .Select(r => new ReferenceAdapter(_entry.Reference(r.Name)))
                            .ToList();
                    }
                }
                return _references;
            }
        }

        private EntryState MapState(EntityState state) => state switch
        {
            EntityState.Added => EntryState.Added,
            EntityState.Modified => EntryState.Modified,
            EntityState.Deleted => EntryState.Deleted,
            _ => EntryState.Unchanged
        };
    }
}
