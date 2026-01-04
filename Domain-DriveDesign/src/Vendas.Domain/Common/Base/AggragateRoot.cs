using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Domain.Common.Base
{
    public abstract class AggragateRoot : Entity
    {
        protected AggragateRoot() : base () 
        { 
        }

        protected AggragateRoot(Guid id) : base(id)
        {
        }
    }
}
