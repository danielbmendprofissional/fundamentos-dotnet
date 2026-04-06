using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Common.Interfaces
{
    public interface IUserProvider
    {
        Guid GetUserId();
        string GetUserName();
        bool IsAuthenticated();
    }
}
