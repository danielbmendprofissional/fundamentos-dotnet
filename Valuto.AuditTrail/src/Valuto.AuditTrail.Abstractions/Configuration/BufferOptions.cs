using System;
using System.Collections.Generic;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Configuration
{
    /// <summary>
    /// Define os limites de recursos e paralelismo do buffer interno.
    /// </summary>
    public class BufferOptions
    {
        /// <summary>
        /// Define a capacidade máxima de itens no canal. 
        /// Ao atingir este limite, a aplicação aplicará Backpressure (aguardará espaço livre).
        /// O valor padrão é 20.000.
        /// </summary>
        public int MaxBufferSize { get; set; } = 20000;

        /// <summary>
        /// Define o número máximo de tarefas de envio simultâneas para o transporte.
        /// Controla o consumo de threads e conexões de rede.
        /// O valor padrão é 10.
        /// </summary>
        public int MaxParallelism { get; set; } = 10;

        /// <summary>
        /// Define a quantidade máxima de registros de auditoria processados em um único lote (Batch).
        /// </summary>
        /// <remarks>
        /// Um valor maior aumenta o throughput de escrita, mas consome mais memória. 
        /// O valor padrão é 100. Deve ser um número positivo.
        /// </remarks>
        public int BatchSize { get; set; } = 100;
    }
}
