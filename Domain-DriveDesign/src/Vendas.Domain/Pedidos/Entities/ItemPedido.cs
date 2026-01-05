using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;
using Vendas.Domain.Pedidos.Interfaces;

namespace Vendas.Domain.Pedidos.Entities
{
    internal sealed class ItemPedido : Entity, IItemPedido
    {/*
        NESSE CASO ESTAMOS REALIZANDO UM SNAPSHOT EM ALGUMAS PROPRIEDADES
        como por NomeProduto, ValorTotal, PrecoUnitario... Isso porque se
        fossemos sempre acessar a propriedade com as informações pelo ID,
        esses valores poderiam ser alterados, como nós queremos manter o 
        histórico, realizamos o SNANPSHOT
      */
        public Guid ProdutoId { get; private set; }
        public string NomeProduto { get; private set; } = string.Empty;
        public decimal PrecoUnitario { get; private set; }
        public int Quantidade { get; private set; }
        public decimal DescontoAplicado { get; private set; }
        public decimal ValorTotal { get; private set; }

        internal ItemPedido(Guid produtoId, string nomeProduto, decimal precoUnitario, int quantidade)
        {
            Guard.AgainstEmptyGuid(produtoId, nameof(produtoId));
            Guard.AgainstNullOrWhiteSpace(nomeProduto, nameof(nomeProduto));
            Guard.Against<DomainException>(precoUnitario <= 0, "O preço unitário deve ser maior que zero.");
            Guard.Against<DomainException>(quantidade <= 0, "A quantidade deve ser maior que zero.");

            ProdutoId = produtoId;
            NomeProduto = nomeProduto;
            PrecoUnitario = precoUnitario;
            Quantidade = quantidade;
            DescontoAplicado = 0;

            CalcularValorTotal();
        }

        internal void AplicarDesconto(decimal desconto)
        {
            Guard.Against<DomainException>(desconto <= 0, "Desconto não pode ser 0 ou negativo.");
            Guard.Against<DomainException>(desconto > PrecoUnitario * Quantidade,
                "Desconto não pode exceder o valor total do item.");

            DescontoAplicado = desconto;
            SetDataAtualizacao();
            CalcularValorTotal();
        }

        internal void AdicionarUnidades(int quantidade)
        {
            Guard.Against<DomainException>(quantidade <= 0, "Deve-se adicionar pelo menos uma unidade.");

            Quantidade += quantidade;
            SetDataAtualizacao();
            CalcularValorTotal();
        }

        internal void RemoverUnidades(int quantidade)
        {
            Guard.Against<DomainException>(quantidade <= 0, "Deve-se remover pelo menos uma unidade.");
            Guard.Against<DomainException>(quantidade > Quantidade,
                "Não é possível remover mais unidades do que existem no item.");

            Quantidade -= quantidade;

            Guard.Against<DomainException>(Quantidade == 0,
                "Um item de pedido não pode ter quantidade zero. Remova-o do pedido.");

            SetDataAtualizacao();
            CalcularValorTotal();
        }

        internal void AtualizarPrecoUnitario(decimal novoPreco)
        {
            Guard.Against<DomainException>(novoPreco <= 0, "O preço unitário deve ser maior que zero.");

            PrecoUnitario = novoPreco;
            SetDataAtualizacao();
            CalcularValorTotal();
        }

        private void CalcularValorTotal()
        {
            ValorTotal = Math.Max(0, PrecoUnitario * Quantidade - DescontoAplicado);
        }
    }
}
