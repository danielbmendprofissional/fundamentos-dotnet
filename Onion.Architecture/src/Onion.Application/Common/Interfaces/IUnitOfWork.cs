using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync(CancellationToken ct = default);
    }
}
