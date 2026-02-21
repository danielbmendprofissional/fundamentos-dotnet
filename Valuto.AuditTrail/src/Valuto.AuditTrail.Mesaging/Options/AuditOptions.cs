namespace Valuto.AuditTrail.Mesaging.Options
{
    internal class AuditOptions
    {
        public string Host { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Queue { get; set; } = "fila-audit-trail-hml";
        public string Exchange { get; set; } = "audit-hml-exchange";
        public string LogQueue { get; set; } = "fila-audit-log";
        public string LogExchange { get; set; } = "audit-log-exchange";
    }
}
