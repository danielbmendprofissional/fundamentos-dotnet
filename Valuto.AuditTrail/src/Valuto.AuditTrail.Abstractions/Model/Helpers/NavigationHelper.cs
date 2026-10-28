using System.Collections.Generic;

namespace Valuto.AuditTrail.Abstractions.Model.Helpers
{
    /// <summary>
    /// Auxiliar para identificação e rastreamento de objetos filhos (navegações) alterados durante o processo de auditoria.
    /// Permite correlacionar entidades dependentes ao objeto raiz.
    /// </summary>
    public class NavigationHelper
    {
        /// <summary>
        /// Estado da entidade filha no ciclo de vida do Entity Framework (ex: Added, Modified, Deleted).
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Identificador único (Chave Primária) da instância filha, convertido para string para transporte.
        /// </summary>
        public string Id { get; set; }
    }
}