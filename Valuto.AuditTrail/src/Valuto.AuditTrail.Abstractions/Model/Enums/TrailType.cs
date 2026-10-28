using System.ComponentModel;

namespace Valuto.AuditTrail.Abstractions.Model.Enums
{
    /// <summary>
    /// Define os tipos de operações que podem ser registradas na trilha de auditoria.
    /// </summary>
    public enum TrailType
    {
        /// <summary>
        /// Operação não definida ou desconhecida.
        /// </summary>
        [Description("None")]
        None = 0,

        /// <summary>
        /// Inserção de um novo registro no banco de dados.
        /// </summary>
        [Description("Create")]
        Create = 1,

        /// <summary>
        /// Alteração de um registro já existente.
        /// </summary>
        [Description("Update")]
        Update = 2,

        /// <summary>
        /// Remoção física ou lógica de um registro.
        /// </summary>
        [Description("Delete")]
        Delete = 3,
    }
}
