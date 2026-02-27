using System.Diagnostics.CodeAnalysis.System.Diagnostics.CodeAnalysis;

namespace Valuto.AuditTrail.Abstractions.Contracts.Auditoria
{
    /// <summary>
    /// Define o contrato para o acesso aos valores e metadados de uma propriedade de entidade.
    /// </summary>
    public interface IAuditProperty
    {
        /// <summary>
        /// Obtém o nome da propriedade conforme definido no modelo de dados.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Obtém o valor atual da propriedade presente na memória do rastreador.
        /// </summary>
        [AllowNull]
        object? CurrentValue { get; }

        /// <summary>
        /// Obtém o valor original da propriedade antes de qualquer modificação na transação atual.
        /// </summary>
        [AllowNull]
        object? OriginalValue { get; }

        /// <summary>
        /// Indica se a propriedade foi marcada como modificada pelo rastreador de alterações.
        /// </summary>
        bool IsModified { get; }

        /// <summary>
        /// Indica se o valor da propriedade é temporário (ex: gerado pelo banco de dados após o SaveChanges).
        /// </summary>
        bool IsTemporary { get; }

        /// <summary>
        /// Armazena os metadados brutos da propriedade (ex: PropertyMetadata ou IProperty).
        /// Deve ser tratado via Reflection pelo adaptador de metadados.
        /// </summary>
        object Metadata { get; }
    }
}
