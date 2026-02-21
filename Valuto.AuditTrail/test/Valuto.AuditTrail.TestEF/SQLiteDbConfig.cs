using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valuto.AuditTrail.TestEF
{
    public class SQLiteDbConfig : DbConfiguration
    {
        public SQLiteDbConfig()
        {
            // OBRIGATORIEDADE: Registrar a Factory e o Service do SQLite de forma pública [cite: 2026-01-31]
            SetProviderFactory("System.Data.SQLite", System.Data.SQLite.SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", System.Data.SQLite.EF6.SQLiteProviderFactory.Instance);

            var providerServices = (DbProviderServices)System.Data.SQLite.EF6.SQLiteProviderFactory.Instance
                .GetService(typeof(DbProviderServices));

            SetProviderServices("System.Data.SQLite", providerServices);
        }
    }
}
