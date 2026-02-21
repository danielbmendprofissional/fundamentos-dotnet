using System;

namespace Valuto.AuditTrail.Abstractions.Commom
{
    /// <summary>
    /// Representa erros de domínio que ocorrem durante a execução do fluxo de auditoria.
    /// </summary>
    /// <remarks>
    /// Esta exceção é utilizada para capturar falhas de regra de negócio ou inconsistências 
    /// de configuração detectadas pelo Nugget.
    /// </remarks>
    public class DomainException : Exception
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="DomainException"/>.
        /// </summary>
        public DomainException() { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="DomainException"/> com uma mensagem de erro específica.
        /// </summary>
        /// <param name="message">A mensagem que descreve o erro.</param>
        public DomainException(string message) : base(message) { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="DomainException"/> com uma mensagem de erro 
        /// e uma referência à exceção interna que é a causa desta exceção.
        /// </summary>
        /// <param name="message">A mensagem que descreve o erro.</param>
        /// <param name="innerException">A exceção que é a causa da exceção atual.</param>
        public DomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}
