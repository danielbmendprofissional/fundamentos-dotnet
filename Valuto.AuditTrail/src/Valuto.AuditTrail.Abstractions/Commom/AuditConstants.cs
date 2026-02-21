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
        public const string PreSaveInicializando = "Pre-Save: Inicializando";

        /// <summary>
        /// Log indicando a captura das entidades no ChangeTracker.
        /// </summary>
        public const string PreSaveCapturando = "Pre-Save: Capturando Tracker";

        /// <summary>
        /// Prefixo para log de mapeamento de metadados de uma entidade específica.
        /// </summary>
        public const string PreSaveMetadados = "Pre-Save: Mapeando metadados de ";

        /// <summary>
        /// Log indicando a criação da instância de trilha de auditoria.
        /// </summary>
        public const string PreSaveCriando = "Pre-Save: Criando Trail";

        /// <summary>
        /// Prefixo para log de extração da chave primária após o salvamento.
        /// </summary>
        public const string PosSavePk = "Pos-Save: Extraindo PK de ";

        /// <summary>
        /// Prefixo para log de resolução da entidade raiz (Aggregate Root).
        /// </summary>
        public const string PosSaveHierarchy = "Pos-Save: Resolvendo Hierarquia (Root) de ";

        /// <summary>
        /// Log indicando a extração de chaves estrangeiras via Reflection para relacionamentos complexos.
        /// </summary>
        public const string PosSaveReflection = "Pos-Save: Extraindo ForeignKeys via Reflection";

        /// <summary>
        /// Log genérico para o processamento final de metadados.
        /// </summary>
        public const string PosSaveMetadados = "Pos-Save: Processamento de Metadados";
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
