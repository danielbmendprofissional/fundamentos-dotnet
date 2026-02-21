namespace Valuto.AuditTrail.Abstractions.Contracts
{
    /// <summary>
    /// Define o provedor responsável por identificar o usuário autor da transação.
    /// </summary>
    /// <remarks>
    /// Esta interface deve ser implementada no projeto cliente para capturar a identidade 
    /// do usuário de acordo com o contexto (ex: HttpContext.User, Environment.UserName, etc).
    /// </remarks>
    public interface IUserProvider
    {
        /// <summary>
        /// Obtém o identificador ou nome do usuário atual no contexto da execução.
        /// </summary>
        /// <returns>Uma string representando o usuário (ex: E-mail, Login ou ID).</returns>
        string GetCurrentUser();
    }
}