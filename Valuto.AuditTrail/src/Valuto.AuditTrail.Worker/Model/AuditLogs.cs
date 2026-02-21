namespace Valuto.AuditTrail.Worker.Model
{
    public sealed class AuditLogs
    {
        public Guid Id { get; private set; }
        public string Mensagem { get; private set; }
        public string StackTrace { get; private set; }
        public string SistemaName { get; private set; }
        public string Contexto { get; private set; }
        public DateTime DateUtc { get; private set; }
        public string VersaoNugget { get; private set; }

        private AuditLogs(
            string mensagem,
            string stackTrace,
            string sistemaName,
            string contexto,
            DateTime dateUtc,
            string versaoNugget)
        {
            Id = Guid.NewGuid();

            Mensagem = mensagem;
            StackTrace = stackTrace;
            SistemaName = sistemaName;
            Contexto = contexto;
            DateUtc = dateUtc;
            VersaoNugget = versaoNugget;
        }

        public static AuditLogs Create(
            string mensagem,
            string stackTrace,
            string sistemaName,
            string contexto,
            DateTime dateUtc,
            string versaoNugget)
                => new AuditLogs(
                    mensagem,
                    stackTrace,
                    sistemaName,
                    contexto,
                    dateUtc,
                    versaoNugget);
    }
}
