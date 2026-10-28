using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Valuto.AuditTrail.Abstractions.Commom;

namespace Valuto.AuditTrail.Abstractions.Model.Events
{
    public class AuditBindingCreatedEvent
    {
        [DataMember(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember(Name = "AuditEntryId")]
        public Guid AuditEntryId { get; set; }

        [DataMember(Name = "TransactionId")]
        public Guid TransactionId { get; set; }

        [DataMember(Name = "PrimaryKey")]
        public string PrimaryKey { get; set; }

        [DataMember(Name = "ForeignKey")]
        public string ForeignKey { get; set; }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "PropertyName")]
        public string PropertyName { get; set; }

        public AuditBindingCreatedEvent()
        {
            Id = Guid.Empty;
            AuditEntryId = Guid.Empty;
            TransactionId = Guid.Empty;
            PrimaryKey = string.Empty;
            ForeignKey = string.Empty;
            Name = string.Empty;
            PropertyName = string.Empty;
        }

        public AuditBindingCreatedEvent(
            Guid id,
            Guid auditEntryId,
            Guid transactionId,
            string primaryKey,
            string foreignKey,
            string name,
            string propertyName) : this()
        {

            Id = id;
            AuditEntryId = auditEntryId;
            TransactionId = transactionId;
            PrimaryKey = primaryKey;
            ForeignKey = foreignKey;
            Name = name;
            PropertyName = propertyName;
        }
    }
}
