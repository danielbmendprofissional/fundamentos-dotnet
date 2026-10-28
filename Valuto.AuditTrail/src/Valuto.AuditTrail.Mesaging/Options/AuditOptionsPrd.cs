using Valuto.AuditTrail.Mesaging.Interfaces;

namespace Valuto.AuditTrail.Mesaging.Options
{
    internal class AuditOptionsPrd : IAuditOptions
    {
        public string Host => "##HOST##";
        public string Username => "##USER##";
        public string Password => "##PASS##";

        public string Queue => "##QUEUEPRD##";
        public string Exchange => "##EXCHPRD##";
        public string LogQueue => "##LOGQPRD##";
        public string LogExchange => "##LOGEPRD##";
    }
}