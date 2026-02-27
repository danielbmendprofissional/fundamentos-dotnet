namespace Valuto.AuditTrail.Mesaging.Options
{
    internal class AuditOptions
    {
        public string Host { get; set; } = "amqp://localhost:5672";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string Queue { get; set; } = "audit-trail-queue";
        public string Exchange { get; set; } = "audit-trail-exchange";
        public string LogQueue { get; set; } = "audit-log-queue";
        public string LogExchange { get; set; } = "audit-log-exchange";
    }
}
