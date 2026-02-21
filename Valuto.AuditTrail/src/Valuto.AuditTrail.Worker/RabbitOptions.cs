namespace Valuto.AuditTrail.Worker
{
    public class RabbitOptions
    {
        public string Host { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Queue { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public string LogQueue { get; set; } = string.Empty;
        public string LogExchange { get; set; } = string.Empty;
    }
}
