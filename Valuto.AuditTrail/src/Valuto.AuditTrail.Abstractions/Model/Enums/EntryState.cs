using System;
using System.Collections.Generic;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Model.Enums
{
    namespace Valuto.AuditTrail.Abstractions.Model.Enums
    {
        /// <summary>
        /// Representa o estado de rastreamento de uma entidade de forma agnóstica ao ORM.
        /// Esta enumeração é utilizada pela Logic para decidir quais mudanças devem ser auditadas.
        /// </summary>
        public enum EntryState
        {
            /// <summary>
            /// A entidade não foi alterada. Corresponde ao estado Unchanged do EF.
            /// </summary>
            Unchanged = 0,
            /// <summary>
            /// A entidade é nova e foi marcada para inserção no banco de dados. Corresponde ao estado Added do EF.
            /// </summary>
            Added = 1,
            /// <summary>
            /// A entidade possui propriedades alteradas que ainda não foram persistidas. Corresponde ao estado Modified do EF.
            /// </summary>
            Modified = 2,
            /// <summary>
            /// A entidade foi marcada para exclusão no banco de dados. Corresponde ao estado Deleted do EF.
            /// </summary>
            Deleted = 4,
        }
    }
}
