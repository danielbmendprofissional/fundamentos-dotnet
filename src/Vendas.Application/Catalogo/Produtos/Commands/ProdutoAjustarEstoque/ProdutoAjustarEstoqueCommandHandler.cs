using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Catalogo.Entities;
using Vendas.Domain.Catalogo.Interfaces.Produtos;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Application.Catalogo.Produtos.Commands.ProdutoAjustarEstoque
{
    public sealed class ProdutoAjustarEstoqueCommandHandler : IRequestHandler<
        ProdutoAjustarEstoqueCommand>
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProdutoAjustarEstoqueCommandHandler(IProdutoRepository produtoRepository, IUnitOfWork unitOfWork)
        {
            _produtoRepository = produtoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ProdutoAjustarEstoqueCommand request, CancellationToken cancellationToken)
        {
            // 1. Busca todos os produtos de uma vez (Performance)
            var ids = request.Produtos.Select(p => p.Id).ToList();
            var entidades = await _produtoRepository.ObterPorIdsAsync(ids, cancellationToken);

            // 2. Validação de Existência (Consistência entre contextos)
            if (entidades.Count() != ids.Count)
            {
                var encontrados = entidades.Select(e => e.Id);
                var faltantes = ids.Except(encontrados);
                // Aqui você decide: Lançar erro ou retornar um objeto de falha
                throw new DomainException($"Os seguintes produtos não existem no catálogo: {string.Join(", ", faltantes)}");
            }

            // 3. Processamento das Regras de Negócio no Domínio
            foreach (var itemRequest in request.Produtos)
            {
                var entity = entidades.First(e => e.Id == itemRequest.Id);

                // A entidade rica valida se há estoque suficiente antes de subtrair
                entity.AjustarEstoque(-itemRequest.Quantidade, "Reserva para pedido");
            }

            // 4. Commit Único (Atomicidade Garantida pelo UoW)
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
