using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Valuto.AuditTrail.Abstractions.Commom;
using Valuto.AuditTrail.Abstractions.Model.Snapshots;

namespace Valuto.AuditTrail.Logic.Extensions
{
    internal static class NavigationExtensions
    {
        public static IEnumerable<string> ObterIdsFormatados(this NavigationSnapshot nav)
        {
            if (nav.Values == null || nav.PrimaryKeys.Count == 0) yield break;

            foreach (var obj in nav.Values)
            {
                if (obj == null) continue;

                if (nav.PrimaryKeys.Count == 1)
                {
                    var prop = obj.GetType().GetProperty(nav.PrimaryKeys[0]);
                    yield return prop?.GetValue(obj)?.ToString() ?? AuditConstants.Unknown;
                }
                else
                {
                    var keys = nav.PrimaryKeys.Select(pk =>
                        obj.GetType().GetProperty(pk)?.GetValue(obj)?.ToString() ?? AuditConstants.Unknown);

                    yield return string.Join("-", keys);
                }
            }
        }
    }
}
