using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Valuto.AuditTrail.EF
{
    internal static class MetadataExtensions
    {
        public static ObjectContext GetObjectContext(this object entry)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            var internalEntry = entry.GetType().GetProperty("InternalEntry", flags)?.GetValue(entry);
            if (internalEntry == null) return null;

            var internalContext = internalEntry.GetType().GetProperty("InternalContext", flags)?.GetValue(internalEntry);
            if (internalContext == null) return null;

            return internalContext.GetType().GetProperty("ObjectContext", flags)?.GetValue(internalContext) as ObjectContext;
        }

        public static EntityType GetCSpaceEntityType(this MetadataWorkspace workspace, Type clrType)
        {
            var entityTypeClr = ObjectContext.GetObjectType(clrType);

            var oSpaceType = workspace.GetItems<EntityType>(DataSpace.OSpace)
                .FirstOrDefault(e => ((ObjectItemCollection)workspace.GetItemCollection(DataSpace.OSpace)).GetClrType(e) == entityTypeClr);

            return oSpaceType != null ? (EntityType)workspace.GetEdmSpaceType(oSpaceType) : null;
        }
    }
}
