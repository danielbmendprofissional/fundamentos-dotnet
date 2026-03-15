namespace Valuto.AuditTrail.Abstractions.Commom
{
    /// <summary>
    /// Centraliza as constantes de texto utilizadas em logs, reflexão e valores padrão em todo o sistema de auditoria.
    /// </summary>
    public static class AuditConstants
    {
        #region LOGS
        /// <summary>
        /// Log indicando o início do processo de auditoria antes da persistência.
        /// </summary>
        public const string Inicializando = "Inicializando auditoria";

        /// <summary>
        /// Log indicando a captura das entidades no ChangeTracker.
        /// </summary>
        public const string Capturando = "Capturando dados do tracker";

        /// <summary>
        /// Prefixo para log de mapeamento de metadados de uma entidade específica.
        /// </summary>
        public const string Criacao = "Criando entidade de auditoria";

        public const string DefinirType = "Definindo type";

        /// <summary>
        /// Log indicando a criação da instância de trilha de auditoria.
        /// </summary>
        public const string DefinirPropriedades = "Definindo propriedades";

        /// <summary>
        /// Prefixo para log de extração da chave primária após o salvamento.
        /// </summary>
        public const string DefinirValoresNavegacao = "Definindo valores de navegação";

        public const string DefinirValoresReferencia = "Definindo valores de referencia";

        /// <summary>
        /// Prefixo para log de resolução da entidade raiz (Aggregate Root).
        /// </summary>
        public const string DefinirRoots = "Definindo valores de Hierarquia (Root)";

        public const string DefinirPrimaryKey = "Definindo valores da chave primaria";
        #endregion

        #region REFLECTION
        /// <summary>
        /// Prefixo utilizado por proxies gerados pelo Castle (Lazy Loading).
        /// </summary>
        public const string Proxies = "Castle.Proxies.";

        /// <summary>
        /// Prefixo utilizado por proxies nativos do EF Core.
        /// </summary>
        public const string EFProxies = "Microsoft.EntityFrameworkCore.Proxies.";
        #endregion

        #region OUTROS
        /// <summary>
        /// Valor padrão para campos não disponíveis ou nulos.
        /// </summary>
        public const string NotAvailable = "N/A";

        /// <summary>
        /// Literal para nome de propriedades.
        /// </summary>
        public const string Name = "Name";

        /// <summary>
        /// Valor padrão para estados ou tipos desconhecidos.
        /// </summary>
        public const string Unknown = "Unknown";

        /// <summary>
        /// Valor padrão para configurações iniciais.
        /// </summary>
        public const string Default = "Default";

        /// <summary>
        /// Texto descritivo para valores de auditoria não preenchidos.
        /// </summary>
        public const string NaoInformado = "Não informado";

        /// <summary>
        /// Texto descritivo para erro ao tentar chamar a base dentro do OnBefore.
        /// </summary>
        public const string RecursionError = "Não é permitido chamar base.SaveChanges dentro do método OnBeforeSaveChanges!";
        #endregion
    }
}
