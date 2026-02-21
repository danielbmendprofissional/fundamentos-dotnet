using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.Abstractions.Model.Events
{
    /// <summary>
    /// Evento de integração que transporta os dados de uma trilha de auditoria para processamento assíncrono.
    /// </summary>
    [DataContract]
    public sealed class AuditTrailCreatedEvent
    {
        /// <summary>
        /// Identificador único do evento. Utilizado como chave primária no banco de dados para garantir idempotência.
        /// </summary>
        [DataMember(Name = "Id")]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Identificador da transação que agrupou as alterações.
        /// </summary>
        [DataMember(Name = "TransactionId")] 
        public Guid TransactionId { get; set; }
        
        /// <summary>
        /// Nome ou identificação do usuário que realizou a operação.
        /// </summary>
        [DataMember(Name = "Usuario")]
        public string Usuario { get; set; }
        
        /// <summary>
        /// Nome da entidade (tabela) que sofreu a alteração.
        /// </summary>
        [DataMember(Name = "EntityName")]
        public string EntityName { get; set; }

        /// <summary>
        /// Nome do sistema de origem que gerou a auditoria.
        /// </summary>
        [DataMember(Name = "SistemaName")]
        public string SistemaName { get; set; }

        /// <summary>
        /// Valor da chave primária do registro auditado.
        /// </summary>
        [DataMember(Name = "PrimaryKey")]
        public string PrimaryKey { get; set; }

        /// <summary>
        /// Nome da entidade raiz em cenários de grafos de objetos.
        /// </summary>
        [DataMember(Name = "Root")]
        public string Root { get; set; }

        /// <summary>
        /// Identificador da entidade raiz.
        /// </summary>
        [DataMember(Name = "RootId")]
        public string RootId { get; set; }

        /// <summary>
        /// Dicionário contendo os valores originais antes da alteração.
        /// </summary>
        [DataMember(Name = "OldValues")]
        public Dictionary<string, object> OldValues { get; set; }

        /// <summary>
        /// Dicionário contendo os novos valores após a alteração.
        /// </summary>
        [DataMember(Name = "NewValues")]
        public Dictionary<string, object> NewValues { get; set; }

        /// <summary>
        /// Lista das propriedades que foram efetivamente alteradas.
        /// </summary>
        [DataMember(Name = "ChangedColumns")]
        public List<string> ChangedColumns { get; set; }

        /// <summary>
        /// Tipo da operação de auditoria (Create, Update, Delete).
        /// </summary>
        [DataMember(Name = "Tipo")]
        public TrailType Tipo { get; set; }

        /// <summary>
        /// Data e hora da criação do evento em formato UTC.
        /// </summary>
        [DataMember(Name = "DateUtc")]
        public DateTime DateUtc { get; set; }
        
        /// <summary>
        /// Inicializa uma nova instância de <see cref="AuditTrailCreatedEvent"/> com coleções instanciadas.
        /// </summary>
        public AuditTrailCreatedEvent()
        {
            OldValues = new Dictionary<string, object>();
            NewValues = new Dictionary<string, object>();
            ChangedColumns = new List<string>();
            Usuario = string.Empty;
            EntityName = string.Empty;
            SistemaName = string.Empty;
            PrimaryKey = string.Empty;
            Root = string.Empty;
            RootId = string.Empty;
        }

        /// <summary>
        /// Inicializa uma instância completa do evento de auditoria.
        /// </summary>
        /// <param name="id">ID único para controle de duplicidade no destino.</param>
        /// <param name="transactionId">ID da transação relacionada.</param>
        /// <param name="usuario">Usuário autor da ação.</param>
        /// <param name="entityName">Nome da entidade auditada.</param>
        /// <param name="sistemaName">Sistema de origem.</param>
        /// <param name="primaryKey">Chave primária do registro.</param>
        /// <param name="oldValues">Valores anteriores.</param>
        /// <param name="newValues">Novos valores.</param>
        /// <param name="changedColumns">Colunas alteradas.</param>
        /// <param name="tipo">Tipo da operação.</param>
        /// <param name="root">Nome da raiz (opcional).</param>
        /// <param name="rootId">ID da raiz (opcional).</param>
        public AuditTrailCreatedEvent(
            Guid id,
            Guid transactionId,
            string usuario,
            string entityName,
            string sistemaName,
            string primaryKey,
            Dictionary<string, object> oldValues,
            Dictionary<string, object> newValues,
            List<string> changedColumns,
            TrailType tipo,
            string root,
            string rootId) : this()
        {
            Id = id;
            TransactionId = transactionId;
            Usuario = usuario;
            EntityName = entityName;
            SistemaName = sistemaName;
            PrimaryKey = primaryKey;
            OldValues = oldValues ?? new Dictionary<string, object>();
            NewValues = newValues ?? new Dictionary<string, object>();
            ChangedColumns = changedColumns ?? new List<string>();
            Tipo = tipo;
            DateUtc = DateTime.UtcNow;
            Root = root;
            RootId = rootId;
        }
    }
}
