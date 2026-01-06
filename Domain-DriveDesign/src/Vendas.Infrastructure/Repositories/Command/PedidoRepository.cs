using Microsoft.EntityFrameworkCore;
using Vendas.Domain.Pedidos.Entities;
using Vendas.Domain.Pedidos.Interfaces;
using Vendas.Infrastructure.Persistence.Command;

namespace Vendas.Infrastructure.Repositories.Command
{
    /*
        REPOSITÓRIO DE AGGREGATE ROOT (DDD)

        - Este repositório é responsável EXCLUSIVAMENTE por persistir e recuperar
          o Aggregate Root Pedido.
        - Ele NÃO expõe entidades internas (ItemPedido, Pagamento).
        - Ele NÃO executa regras de negócio.
        - Ele NÃO realiza commit (SaveChanges).
        - Ele NÃO é usado para queries de leitura (CQRS).
    */
    internal sealed class PedidoRepository : IPedidoRepository
    {
        private readonly VendasDbContext _context;

        /*
            O repositório depende APENAS do DbContext.
            Não injeta outros repositórios.
            Não injeta UnitOfWork.
            Isso evita acoplamento e mantém o repositório focado.
        */
        public PedidoRepository(VendasDbContext context)
        {
            _context = context;
        }

        /*
            Recupera o Aggregate Root completo.

            IMPORTANTE (DDD):
            - Sempre retorna o Aggregate inteiro.
            - Nunca retorna entidades internas isoladas.
            - Nunca retorna projeções ou DTOs.
            - Queries ricas NÃO pertencem ao repositório de escrita.
        */
        public async Task<Pedido?> ObterPorIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Pedidos
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        /*
            Adiciona um novo Aggregate Root ao contexto.

            IMPORTANTE:
            - Não chama SaveChanges.
            - Apenas registra o aggregate no ChangeTracker.
            - O commit é responsabilidade do UnitOfWork.
        */
        public async Task AdicionarAsync(
            Pedido pedido,
            CancellationToken cancellationToken = default)
        {
            await _context.Pedidos.AddAsync(pedido, cancellationToken);
        }

        /*
            Marca o Aggregate como modificado.

            IMPORTANTE:
            - Não é async (não há I/O).
            - Não executa persistência.
            - Apenas informa ao EF que o estado mudou.
        */
        public void Atualizar(Pedido pedido)
        {
            _context.Pedidos.Update(pedido);
        }
    }
}
