using System;
using Valuto.AuditTrail.Abstractions.Commom;

namespace Valuto.AuditTrail.Abstractions.Model.Events
{
    /// <summary>
    /// Representa o evento de log técnico enviado pelo Nugget para processamento assíncrono.
    /// Utilizado para registrar exceções, erros de sistema ou mensagens informativas de auditoria.
    /// </summary>
    public sealed class AuditLogEvent
    {
        /// <summary>
        /// Descrição detalhada da mensagem de log ou do erro ocorrido.
        /// </summary>
        public string Mensagem { get; set; }

        /// <summary>
        /// Rastro da pilha de execução (Stack Trace) em caso de exceções, facilitando o debug.
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Nome do sistema ou aplicação que gerou o log.
        /// </summary>
        public string SistemaName { get; set; }

        /// <summary>
        /// Contexto de negócio relacionado ao evento (ex: Nome da Tabela ou Classe).
        /// </summary>
        public string Contexto { get; set; }

        /// <summary>
        /// Data e hora da ocorrência em formato UTC.
        /// </summary>
        public DateTime DateUtc { get; set; }

        /// <summary>
        /// Versão do pacote Nugget que gerou o log, útil para rastreabilidade de compatibilidade.
        /// </summary>
        public string VersaoNugget { get; set; }

        /// <summary>
        /// Inicializa uma nova instância de <see cref="AuditLogEvent"/> com valores padrão.
        /// </summary>
        public AuditLogEvent()
        {
            DateUtc = DateTime.UtcNow;
            VersaoNugget = "1.0.0";
        }

        /// <summary>
        /// Inicializa uma instância de log com os dados principais da ocorrência.
        /// </summary>
        /// <param name="mensagem">Mensagem descritiva do log.</param>
        /// <param name="stackTrace">Rastro da pilha de erro (opcional).</param>
        /// <param name="sistemaName">Nome do sistema de origem.</param>
        /// <param name="entityName">Contexto ou nome da entidade relacionada.</param>
        public AuditLogEvent(
            string mensagem,
            string stackTrace,
            string sistemaName,
            string entityName = AuditConstants.NotAvailable) : this()
        {
            Mensagem = mensagem;
            StackTrace = stackTrace;
            SistemaName = sistemaName;
            Contexto = entityName;
        }
    }
}
