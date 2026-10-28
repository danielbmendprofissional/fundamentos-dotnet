using System;
using System.Collections.Generic;
using System.Text;

namespace Valuto.AuditTrail.Mesaging.Interfaces
{
    interface IAuditOptions
    {
        string Host { get; }
        string Username { get; }
        string Password { get; }

        string Queue { get; }
        string Exchange { get; }
        string LogQueue { get; }
        string LogExchange { get; }
    }
}
