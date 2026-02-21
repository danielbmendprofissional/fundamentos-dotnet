using MassTransit;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Valuto.AuditTrail.Abstractions.Model.Entities;
using Valuto.AuditTrail.Abstractions.Model.Events;
using Valuto.AuditTrail.Worker.Interfaces;

namespace Valuto.AuditTrail.Worker
{
    public class AuditTrailConsumer(IAuditTrailsRepository repository) : IConsumer<AuditTrailCreatedEvent>
    {
        public async Task Consume(ConsumeContext<AuditTrailCreatedEvent> context)
        {
            var message = context.Message;

            var auditEntity = AuditTrails.Criar(
                    message.Usuario, 
                    message.SistemaName, 
                    message.EntityName, 
                    message.Id);

            auditEntity.DefinirTransactionId(message.TransactionId);

            auditEntity.DefinirPrimaryKey(message.PrimaryKey);
            auditEntity.DefinirTrailType(message.Tipo);
            auditEntity.DefinirRoot(message.Root, message.RootId);

            message.OldValues?.ToList().ForEach(x => auditEntity.AdicionarOldValues(x.Key, x.Value));
            message.NewValues?.ToList().ForEach(x => auditEntity.AdicionarNewValues(x.Key, x.Value));
            message.ChangedColumns?.ToList().ForEach(auditEntity.AdicionarChangedColumns);

            try
            {
                await repository.AdicionarUnicoAsync(auditEntity);
            }
            catch (DbUpdateException ex)
            {
                // ORA-00001
                if (ex.InnerException is OracleException oraEx && oraEx.Number == 1)
                {
                    return;
                }
                throw;
            }
        }
    }
}
