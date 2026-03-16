using System;
using System.Collections.Generic;
using System.Text;
using Valuto.AuditTrail.Abstractions.Commom;

namespace Valuto.AuditTrail.Abstractions.Model.Entities
{
    public sealed class AuditBinding
    {
        public Guid Id { get; private set; }
        public Guid AuditEntryId { get; private set; }
        public Guid TransactionId { get; set; }


        public string PrimaryKey { get; private set; } = string.Empty;
        public string ForeignKey { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string PropertyName { get; private set; } = string.Empty;

        internal AuditBinding(
            Guid auditEntryId, 
            Guid transactionId,
            string primaryKey, 
            string foreignKey, 
            string name, 
            string propertyName,
            Guid? id = null) 
        {

            Guard.AgainstEmptyGuid(auditEntryId, nameof(auditEntryId));
            Id = id ?? Guid.NewGuid();

            AuditEntryId = auditEntryId;
            TransactionId = transactionId;
            PrimaryKey = primaryKey;
            ForeignKey = foreignKey;
            Name = name;
            PropertyName = propertyName;
        }
    }
}
