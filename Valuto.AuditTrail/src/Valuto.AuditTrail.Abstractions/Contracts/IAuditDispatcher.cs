using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Valuto.AuditTrail.Abstractions.Contracts
{
    /// <summary>
    /// OBRIGATORIEDADE: Interface para desacoplamento do motor de despacho.
    /// Define o contrato para o serviço que consome o buffer e realiza a publicação dos lotes.
    /// </summary>
    /// <typeparam name="T">O tipo da entidade de auditoria.</typeparam>
    public interface IAuditDispatcher<T> where T : class
    {
        /// <summary>
        /// Inicia o processamento assíncrono do motor de auditoria.
        /// Teoria na Prática: Este método é o ponto de entrada para o consumo do <see cref="IAuditBuffer{T}"/>.
        /// Em ambientes com Host (ASP.NET Core), ele é gerenciado pelo ciclo de vida do IHostedService.
        /// Em ambientes Legados/Console, pode ser disparado via gatilho no primeiro Enqueue.
        /// </summary>
        /// <param name="ct">Token de cancelamento para interromper o motor de despacho.</param>
        /// <returns>Uma <see cref="Task"/> que representa a execução contínua do motor.</returns>
        Task RunAsync(CancellationToken ct);
    }
}
