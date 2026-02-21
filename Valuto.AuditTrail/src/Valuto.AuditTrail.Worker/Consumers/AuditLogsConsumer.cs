using MassTransit;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Valuto.AuditTrail.Abstractions.Model.Events;
using Valuto.AuditTrail.Worker.Interfaces;
using Valuto.AuditTrail.Worker.Model;

namespace Valuto.AuditTrail.Worker.Consumers
{
    public class AuditLogsConsumer(IAuditLogsRepository repository) : IConsumer<AuditLogEvent>
    {
        public async Task Consume(ConsumeContext<AuditLogEvent> context)
        {
            var erro = context.Message;

            var auditLogs = AuditLogs.Create(
                                    erro.Mensagem,
                                    erro.StackTrace,
                                    erro.SistemaName,
                                    erro.Contexto,
                                    erro.DateUtc,
                                    erro.VersaoNugget);

            try
            {
                await repository.AdicionarUnicoAsync(auditLogs);
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
