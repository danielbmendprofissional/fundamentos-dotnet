using System;
using System.Collections.Generic;
using System.Text;
using Valuto.AuditTrail.Abstractions.Model.Enums;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.Logic.Extensions
{
    internal static class EntryStateExtensions
    {
        public static TrailType ObterTipoTrilha(this EntryState state)
        {
            switch (state)
            {
                case EntryState.Added: return TrailType.Create;
                case EntryState.Deleted: return TrailType.Delete;
                case EntryState.Modified: return TrailType.Update;
                default: return TrailType.None;
            }
        }
    }
}
