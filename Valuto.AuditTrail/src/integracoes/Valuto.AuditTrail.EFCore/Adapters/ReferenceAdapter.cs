using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria.Adapters;

namespace Valuto.AuditTrail.EFCore.Adapters
{
    internal class ReferenceAdapter : IReferenceAdapter
    {
        private readonly NavigationEntry _navigation;
        public ReferenceAdapter(NavigationEntry navigation)
        {
            _navigation = navigation;
        }

        public string Name => _navigation.Metadata.Name;

        public bool IsModified => _navigation.IsModified;
    }
}
