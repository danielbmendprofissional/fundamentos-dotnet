using System;
using System.Collections.Generic;
using System.Text;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots
{
    public sealed class AuditPropertySnapshot : IAuditProperty
    {
        public string Name { get; set; }
        public object OriginalValue { get; set; }

        private Func<object> _valueProvider;
        private object _staticCurrentValue;

        public object CurrentValue
        {
            get => IsTemporary ? _valueProvider?.Invoke() : _staticCurrentValue;
            set => _staticCurrentValue = value;
        }

        public bool IsTemporary { get; set; }
        public bool IsModified { get; set; }
        public object Metadata { get; set; }

        public void SetLazyLoader(Func<object> loader)
        {
            _valueProvider = loader;
            IsTemporary = true;
        }
    }
}
