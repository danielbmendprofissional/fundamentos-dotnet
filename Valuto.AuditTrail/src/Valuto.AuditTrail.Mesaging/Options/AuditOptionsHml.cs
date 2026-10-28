using System;
using System.Collections.Generic;
using System.Text;
using Valuto.AuditTrail.Mesaging.Interfaces;

namespace Valuto.AuditTrail.Mesaging.Options
{
    internal class AuditOptionsHml : IAuditOptions
    {
        public string Host => "amqp://localhost:5672";
        public string Username => "guest";
        public string Password => "guest";

        public string Queue => "fila-audit-trail-hml";
        public string Exchange => "audit-hml-exchange";
        public string LogQueue => "fila-audit-log-hml";
        public string LogExchange => "audit-hml-log-exchange";
    }
}