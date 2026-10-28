using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria.Adapters;

namespace Valuto.AuditTrail.EF.Adapters
{
    internal class ReferenceAdapter : IReferenceAdapter
    {
        private readonly DbEntityEntry _parentEntry;
        private readonly string _navigationName;
        private readonly List<string> _dependentPropertyNames;

        public ReferenceAdapter(
            DbEntityEntry parentEntry,
            string navigationName,
            IEnumerable<string> dependentPropertyNames)
        {
            _parentEntry = parentEntry;
            _navigationName = navigationName;
            _dependentPropertyNames = dependentPropertyNames?.ToList() ?? new List<string>();
        }

        public string Name => _navigationName;

        public bool IsModified
        {
            get
            {
                if (_parentEntry == null || _dependentPropertyNames.Count == 0)
                    return false;

                return _dependentPropertyNames.Any(propName =>
                    _parentEntry.Property(propName).IsModified);
            }
        }
    }
}
