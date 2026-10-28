using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Valuto.AuditTrail.Logic.Paralel
{
    /// <summary>
    /// Interface responsável pelo gerenciamento de memória e desacoplamento entre a produção de logs e o despacho.
    /// OBRIGATORIEDADE: Esta interface deve ser registrada como Singleton para garantir a integridade do Channel.
    /// </summary>
    /// <typeparam name="T">O tipo da entidade de auditoria.</typeparam>
    internal interface IAuditBuffer<T> where T : class
    {
        /// <summary>
        /// Adiciona um item ao buffer de forma assíncrona. 
        /// Se o buffer estiver cheio (Bounded), aguarda a liberação de espaço sem bloquear a thread.
        /// </summary>
        /// <param name="message">A mensagem de auditoria a ser processada.</param>
        /// <param name="cancellationToken">Token para cancelamento da operação de escrita.</param>
        /// <returns>Uma <see cref="ValueTask"/> representando a operação de enfileiramento.</returns>
        ValueTask EnqueueAsync(T message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fornece uma leitura contínua e assíncrona dos dados presentes no buffer.
        /// OBRIGATORIEDADE: O enumerador deve permanecer ativo até que o método <see cref="Complete"/> seja chamado.
        /// </summary>
        /// <param name="cancellationToken">Token para sinalizar a interrupção da leitura.</param>
        /// <returns>Uma sequência enumerável assíncrona de mensagens.</returns>
        IAsyncEnumerable<T> ReadAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Tenta remover e retornar o objeto no início do buffer de forma não-bloqueante (Thread-Safe).
        /// </summary>
        /// <param name="message">O item removido, ou null caso o buffer esteja vazio.</param>
        /// <returns>True se um item foi lido com sucesso; caso contrário, false.</returns>
        bool TryRead(out T message);

        /// <summary>
        /// Sinaliza que nenhum novo item será adicionado ao buffer.
        /// Teoria na Prática: Permite o encerramento gracioso (Graceful Shutdown) do Dispatcher, 
        /// finalizando o loop de leitura após o esvaziamento do canal.
        /// </summary>
        void Complete();
    }
}
