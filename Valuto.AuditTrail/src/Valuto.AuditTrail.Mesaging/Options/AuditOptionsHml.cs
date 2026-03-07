using System;
using System.Collections.Generic;
using System.Text;
using Valuto.AuditTrail.Mesaging.Interfaces;

namespace Valuto.AuditTrail.Mesaging.Options
{
    internal class AuditOptionsHml : IAuditOptions
    {
        public string Host => "##HOST##";
        public string Username => "##USER##";
        public string Password => "##PASS##";

        public string Queue => "##QUEUEHML##";
        public string Exchange => "##EXCHHML##";
        public string LogQueue => "##LOGQHML##";
        public string LogExchange => "##LOGEHML##";
    }
}