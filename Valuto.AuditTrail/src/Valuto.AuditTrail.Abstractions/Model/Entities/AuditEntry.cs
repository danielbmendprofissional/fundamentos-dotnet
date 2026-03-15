using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Valuto.AuditTrail.Abstractions.Commom;
using Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.Abstractions.Model.Entities
{
    /// <summary>
    /// Representa um registro detalhado de alteração de estado de uma entidade (Trilha de Auditoria).
    /// </summary>
    public sealed class AuditEntry
    {
        /// <summary>
        /// Identificador único do rastro de auditoria. 
        /// Pode ser gerado automaticamente ou fornecido para garantir idempotência em reprocessamentos.
        /// </summary>
        public Guid Id { get; private set; }
        /// <summary>
        /// Identificador da transação que agrupou esta e outras alterações relacionadas.
        /// </summary>
        public Guid TransactionId { get; private set; }
        /// <summary>
        /// Identificação do usuário responsável pela alteração.
        /// </summary>
        public string Usuario { get; private set; }
        /// <summary>
        /// Tipo da operação realizada (Insert, Update, Delete, etc).
        /// </summary>
        public TrailType Tipo { get; private set; }
        /// <summary>
        /// Data e hora do evento em formato UTC.
        /// </summary>
        public DateTime DateUtc { get; private set; }
        /// <summary>
        /// Nome do sistema ou aplicação de origem do evento.
        /// </summary>
        public string SistemaName { get; set; }
        /// <summary>
        /// Nome da entidade que sofreu a alteração.
        /// </summary>
        public string EntityName { get; private set; }
        /// <summary>
        /// Valor da chave primária da entidade auditada (geralmente em formato string ou JSON).
        /// </summary>
        [AllowNull]
        public string? PrimaryKey { get; private set; }

        [AllowNull]
        public IReadOnlyCollection<AuditBinding>? AuditBindings => _auditBinding?.AsReadOnly();
        [AllowNull]
        private List<AuditBinding>? _auditBinding = new List<AuditBinding>();

        /// <summary>
        /// Coleção de valores antes da alteração.
        /// </summary>
        [AllowNull]
        public IReadOnlyDictionary<string, object>? OldValues => _oldValues;
        [AllowNull]
        private Dictionary<string, object>? _oldValues = new Dictionary<string, object>();
        
        /// <summary>
        /// Coleção de valores após a alteração.
        /// </summary>
        [AllowNull]
        public IReadOnlyDictionary<string, object>? NewValues => _newValues;
        [AllowNull]
        private Dictionary<string, object>? _newValues = new Dictionary<string, object>();

        /// <summary>
        /// Lista das colunas que sofreram modificação nesta transação.
        /// </summary>
        [AllowNull]
        public IReadOnlyCollection<string>? ChangedColumns => _changedColumns?.AsReadOnly();
        [AllowNull]
        private List<string>? _changedColumns = new List<string>();

        private AuditEntry(string usuario, string sistemaName, string entityName, Guid? id = null)
        {
            Guard.AgainstNullOrWhiteSpace(entityName, nameof(entityName));

            Id = id ?? Guid.NewGuid();

            DateUtc = DateTime.UtcNow;
            Usuario = usuario;
            Tipo = TrailType.None;
            EntityName = entityName;
            SistemaName = sistemaName;
        }

        private AuditEntry() { }
        /// <summary>
        /// Cria uma nova instância da auditoria.
        /// </summary>
        /// <param name="usuario">Identificador do usuário.</param>
        /// <param name="sistemaName">Nome do sistema de origem.</param>
        /// <param name="entityName">Nome da entidade auditada.</param>
        /// <param name="id">ID fixo para idempotência. Se nulo, um novo Guid será gerado.</param>
        public static AuditEntry Criar(string usuario, string sistemaName, string entityName, Guid? id = null)
            => new AuditEntry(usuario, sistemaName, entityName, id);
        /// <summary>
        /// Define o vínculo da auditoria com uma transação específica.
        /// </summary>
        public void DefinirTransactionId(Guid transactionId)
        {
            Guard.AgainstEmptyGuid(transactionId, nameof(transactionId));
            TransactionId = transactionId;
        }
        /// <summary>
        /// Registra o tipo da operação de transação.
        /// </summary>
        public void DefinirTrailType(TrailType tipo)
        {
            Guard.AgainstInvalidEnum<TrailType>(tipo, nameof(Tipo));
            Tipo = tipo;
        }
        /// <summary>
        /// Registra a primary key referente ao objeto de criação no banco do cliente.
        /// </summary>
        public void DefinirPrimaryKey(string valor)
        {
            PrimaryKey = valor;
        }

        public void AdicionarBinding(
            Guid transactionId,
            string primaryKey,
            string foreignKey,
            string name,
            string propertyName)
        {
            if (_auditBinding != null)
            {
                _auditBinding.Add(new AuditBinding(
                    Id,
                    transactionId,
                    primaryKey, 
                    foreignKey, 
                    name, 
                    propertyName));
            }
        }

        /// <summary>
        /// Registra os valores originais da entidade antes da persistência, em caso de atualização
        /// </summary>
        public void AdicionarOldValues(string key, object value)
        {
            Guard.AgainstNullOrWhiteSpace(key, nameof(key));
            if (_oldValues != null)
            {
                _oldValues[key] = value;
            }
        }

        /// <summary>
        /// Registra os novos valores da entidade após a modificação ou criação.
        /// </summary>
        public void AdicionarNewValues(string key, object value)
        {
            Guard.AgainstNullOrWhiteSpace(key, nameof(key));
            if (_newValues != null)
            {
                _newValues[key] = value;
            }
        }

        /// <summary>
        /// Registra mudanças internas de uma coluna da entidade auditada
        /// </summary>
        public void AdicionarChangedColumns(string valor)
        {
            if (_changedColumns != null && !_changedColumns.Contains(valor))
            {
                _changedColumns.Add(valor);
            }
        }
    }
}
