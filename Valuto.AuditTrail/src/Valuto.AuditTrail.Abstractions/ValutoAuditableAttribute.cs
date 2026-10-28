using System;

namespace Valuto.AuditTrail.Abstractions
{
    /// <summary>
    /// Define os estados de persistência que serão monitorados para uma entidade específica.
    /// Por possuir o atributo [Flags], permite combinações bitwise dos estados.
    /// </summary>
    [Flags]
    public enum AuditoriaEstado
    {
        /// <summary>
        /// Monitora a inserção de novos registros.
        /// </summary>
        Create = 1,

        /// <summary>
        /// Monitora a alteração de registros existentes.
        /// </summary>
        Update = 2,

        /// <summary>
        /// Monitora a exclusão de registros.
        /// </summary>
        Delete = 4
    }

    /// <summary>
    /// Atributo de marcação utilizado para indicar que uma classe (entidade) deve ser auditada pelo Nugget.
    /// Permite configurar quais operações (Create, Update, Delete) devem disparar o rastro de auditoria.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ValutoAuditableAttribute : Attribute
    {
        /// <summary>
        /// Obtém os estados de auditoria configurados para a entidade.
        /// </summary>
        public AuditoriaEstado Estados { get; }

        /// <summary>
        /// Inicializa uma nova instância do atributo definindo quais estados serão auditados.
        /// O padrão, caso não informado, é auditar todas as operações (Create, Update e Delete).
        /// </summary>
        /// <param name="estados">Combinação de <see cref="AuditoriaEstado"/> a serem monitorados.</param>
        public ValutoAuditableAttribute(AuditoriaEstado estados = AuditoriaEstado.Create | AuditoriaEstado.Update | AuditoriaEstado.Delete)
        {
            Estados = estados;
        }
    }
}
