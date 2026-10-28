using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Valuto.AuditTrail.Abstractions.Model.Snapshots;

namespace Valuto.AuditTrail.Logic.Extensions
{
    public static class PropertyExtensions
    {
        /// <summary>
        /// Consolida as chaves primárias em uma string única formatada.
        /// Ex: "101" ou "101|202" para chaves compostas.
        /// </summary>
        public static string MontarChavePrimaria(this IEnumerable<PropertySnapshot> properties)
        {
            var keys = properties.Where(p => p.IsPrimaryKey && p.CurrentValue != null).ToList();

            if (keys.Count == 0) return string.Empty;

            if (keys.Count == 1) return keys[0].CurrentValue.ToString();

            var compositeId = new StringBuilder();
            for (int i = 0; i < keys.Count; i++)
            {
                if (i > 0) compositeId.Append("|");
                compositeId.Append(keys[i].CurrentValue);
            }

            return compositeId.ToString();
        }
    }
}
