using FluentAssertions;
using System.Reflection;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Pedidos.Entities;
using Vendas.Domain.Pedidos.Enums;
using Vendas.Domain.Pedidos.Events.Pedido;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Domain.Tests.Pedidos.Entities
{
    public class PedidoTests
    {
        private static EnderecoEntrega CriarEnderecoValido()
            => EnderecoEntrega.Criar("12345-000", "Rua A", "Ap 1", "Centro", "SP", "São Paulo", "Brasil");

        private static readonly Guid ClienteIdValido = Guid.NewGuid();
        private static readonly Guid ProdutoIdValido = Guid.NewGuid();

        private static void SetStatusPedido(Pedido pedido, StatusPedido status)
        {
            typeof(Pedido)
                .GetProperty(nameof(Pedido.StatusPedido),
                    BindingFlags.Public | BindingFlags.Instance)!
                .SetValue(pedido, status);
        }


        #region FACT -> SHOULD
        #endregion

        #region FACT -> SHOULD NOT
        #endregion

        #region THEORY -> SHOULD
        #endregion

        [Fact(DisplayName = "Should create a valid order with Pending status")]
        public void Should_Create_Valid_Order()
        {
            // Act
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            // Assert
            pedido.Should().NotBeNull();
            pedido.ClienteId.Should().Be(ClienteIdValido);
            pedido.EnderecoEntrega.Should().NotBeNull();
            pedido.StatusPedido.Should().Be(StatusPedido.Pendente);
            pedido.ValorTotal.Should().Be(0);
            pedido.Itens.Should().BeEmpty();
            pedido.Pagamentos.Should().BeEmpty();
            pedido.Id.Should().NotBeEmpty();
        }

        [Fact(DisplayName = "Should not create order with invalid ClientId")]
        public void Should_Not_Create_Order_With_Invalid_ClientId()
        {
            // Act
            Action act = () => Pedido.Criar(Guid.Empty, CriarEnderecoValido());

            // Assert
            act.Should()
               .Throw<DomainException>()
               .WithMessage("clienteId cannot be Guid.empty");
        }

        [Fact(DisplayName = "Should not create order without delivery address")]
        public void Should_Not_Create_Order_Without_Address()
        {
            // Act
            Action act = () => Pedido.Criar(ClienteIdValido, null!);

            // Assert
            act.Should()
               .Throw<DomainException>()
               .WithMessage("enderecoEntrega cannot be null");
        }

        // items

        [Fact(DisplayName = "Should add item to order and recalculate total value")]
        public void Deve_Adicionar_Item_Ao_Pedido()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());

            // Act
            pedido.AdicionarItem(ProdutoIdValido, "Mouse", 100m, 2);

            // Assert
            pedido.Itens.Should().HaveCount(1);
            pedido.ValorTotal.Should().Be(200m);
            pedido.Itens.First().ValorTotal.Should().Be(200m);
        }

        [Fact(DisplayName = "Should sum quantity of existing item when adding the same product")]
        public void Deve_Somar_Quantidade_De_Item_Existente()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            var produtoId = ProdutoIdValido;

            // Act
            pedido.AdicionarItem(produtoId, "Teclado", 200m, 1);
            pedido.AdicionarItem(produtoId, "Teclado", 200m, 2);

            // Assert
            pedido.Itens.Should().HaveCount(1);
            var item = pedido.Itens.First();
            item.Quantidade.Should().Be(3);
            item.ValorTotal.Should().Be(600m);
            pedido.ValorTotal.Should().Be(600m);
        }

        [Theory(DisplayName = "Should not allow adding items when order is not Pending")]
        [InlineData(StatusPedido.PagamentoConfirmado)]
        [InlineData(StatusPedido.EmSeparacao)]
        [InlineData(StatusPedido.Enviado)]
        [InlineData(StatusPedido.Entregue)]
        [InlineData(StatusPedido.Cancelado)]
        public void Nao_Deve_Adicionar_Item_Quando_Pedido_Nao_Estiver_Pendente(StatusPedido status)
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            SetStatusPedido(pedido, status);

            // Act
            Action act = () => pedido.AdicionarItem(Guid.NewGuid(), "Outro", 100m, 1);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Itens só podem ser adicionados enquanto o pedido está pendente.");
        }

        [Fact(DisplayName = "Should remove item and recalculate total value")]
        public void Deve_Remover_Item_E_Recalcular_Valor()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Mouse", 100m, 2);

            // Act
            Action act = () => pedido.RemoverItem(pedido.Itens.First().Id);

            // Assert
            act.Should().Throw<DomainException>()
               .WithMessage("O pedido deve conter pelo menos um item.");
        }

        [Fact(DisplayName = "Should remove item and recalculate total value when there is more than one item")]
        public void Deve_Remover_Item_Quando_Houver_Mais_De_Um()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            var produto1 = Guid.NewGuid();
            var produto2 = Guid.NewGuid();

            pedido.AdicionarItem(produto1, "Mouse", 100m, 1);
            pedido.AdicionarItem(produto2, "Teclado", 200m, 1);

            var itemId = pedido.Itens.First(i => i.ProdutoId == produto1).Id;

            // Act
            pedido.RemoverItem(itemId);

            // Assert
            pedido.Itens.Should().HaveCount(1);
            pedido.ValorTotal.Should().Be(200m);
        }

        [Fact(DisplayName = "Should ignore removal of non-existent item")]
        public void Deve_Ignorar_Remocao_De_Item_Inexistente()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Mouse", 100m, 2);

            // Act
            Action act = () => pedido.RemoverItem(Guid.NewGuid());

            // Assert
            act.Should().Throw<DomainException>()
               .WithMessage("item cannot be null");
        }

        [Theory(DisplayName = "Não deve permitir remover itens quando pedido não estiver Pendente")]
        [InlineData(StatusPedido.PagamentoConfirmado)]
        [InlineData(StatusPedido.EmSeparacao)]
        [InlineData(StatusPedido.Enviado)]
        [InlineData(StatusPedido.Entregue)]
        [InlineData(StatusPedido.Cancelado)]
        public void Nao_Deve_Remover_Item_Quando_Nao_Pendente(StatusPedido status)
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto", 10m, 1);
            SetStatusPedido(pedido, status);

            // Act
            Action act = () => pedido.RemoverItem(ProdutoIdValido);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Itens só podem ser removidos em pedidos pendentes.");
        }

        // endereco
        [Fact(DisplayName = "Deve atualizar endereço de entrega quando Pendente")]
        public void Deve_Atualizar_Endereco_Quando_Pendente()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            var novoEndereco = CriarEnderecoValido();

            // Act
            pedido.AtualizarEnderecoEntrega(novoEndereco);

            // Assert
            pedido.EnderecoEntrega.Should().Be(novoEndereco);
        }

        [Theory(DisplayName = "Não deve atualizar endereço de entrega quando não Pendente")]
        [InlineData(StatusPedido.PagamentoConfirmado)]
        [InlineData(StatusPedido.EmSeparacao)]
        [InlineData(StatusPedido.Enviado)]
        [InlineData(StatusPedido.Entregue)]
        [InlineData(StatusPedido.Cancelado)]
        public void Nao_Deve_Atualizar_Endereco_Quando_Nao_Pendente(StatusPedido status)
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            var novoEndereco = CriarEnderecoValido();
            SetStatusPedido(pedido, status);

            // Act
            Action act = () => pedido.AtualizarEnderecoEntrega(novoEndereco);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("O endereço só pode ser alterado enquanto o pedido está pendente.");
        }

        // pagamentos
        [Fact(DisplayName = "Deve iniciar pagamento e manter status Pendente")]
        public void Deve_Iniciar_Pagamento()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto", 100m, 2);

            // Act
            var pagamento = pedido.IniciarPagamento(MetodoPagamento.CartaoCredito);

            // Assert
            pedido.Pagamentos.Should().Contain(x => x.Id == pagamento);
            pedido.StatusPedido.Should().Be(StatusPedido.Pendente);
        }

        [Fact(DisplayName = "Não deve iniciar pagamento sem itens no pedido")]
        public void Nao_Deve_Iniciar_Pagamento_Sem_Itens()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());

            // Act
            Action act = () => pedido.IniciarPagamento(MetodoPagamento.Pix);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Não é possível iniciar o pagamento de um pedido sem itens.");
        }

        [Fact(DisplayName = "Não deve iniciar pagamento se já houver um pagamento Pendente")]
        public void Nao_Deve_Iniciar_Pagamento_Se_Ja_Houver_Pendente()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto", 100m, 1);

            // Simula a criação de um pagamento Pendente 
            pedido.IniciarPagamento(MetodoPagamento.Pix);

            // Act
            Action act = () => pedido.IniciarPagamento(MetodoPagamento.CartaoCredito);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Já existe um pagamento pendente para este pedido.");
        }

        [Fact(DisplayName = "Deve alterar status para PagamentoConfirmado ao ConfirmarPagamento")]
        public void Deve_Alterar_Status_Ao_HandlePagamentoAprovado()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto", 100m, 1);
            var pagamento = pedido.IniciarPagamento(MetodoPagamento.Pix);
            pedido.DefinirCodigoTransacao(pagamento);

            // Act
            pedido.ConfirmarPagamento(pagamento);

            // Assert
            pedido.StatusPedido.Should().Be(StatusPedido.PagamentoConfirmado);
        }

        [Fact(DisplayName = "Deve manter status Pendente ao RecusarPagamento")]
        public void Deve_Manter_Status_Pendente_Ao_HandlePagamentoRecusado()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto", 100m, 1);
            var pagamento = pedido.IniciarPagamento(MetodoPagamento.Pix);
            pedido.DefinirCodigoTransacao(pagamento);

            // Act
            pedido.RecusarPagamento(pagamento);

            // Assert
            pedido.StatusPedido.Should().Be(StatusPedido.Cancelado);
        }

        [Fact(DisplayName = "Não deve ConfirmarPagamento se status não for Pendente")]
        public void Nao_Deve_HandlePagamentoAprovado_Se_Nao_Pendente()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto", 100m, 1);
            var pagamento = pedido.IniciarPagamento(MetodoPagamento.Pix);
            SetStatusPedido(pedido, StatusPedido.EmSeparacao); // Simula status incorreto

            // Act
            Action act = () => pedido.ConfirmarPagamento(pagamento);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("O pedido não está no status esperado para confirmação de pagamento.");
        }

        // transição de estado

        //[Fact(DisplayName = "Deve permitir marcar pedido como Em Separacao após PagamentoConfirmado")]
        //public void Deve_Marcar_Como_Em_Separacao()
        //{
        //    // Arrange
        //    var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
        //    pedido.AdicionarItem(ProdutoIdValido, "Produto", 100m, 1);
        //    var pagamento = pedido.IniciarPagamento(MetodoPagamento.CartaoCredito);
        //    pedido.DefinirCodigoTransacao(pagamento);

        //    pedido.ConfirmarPagamento(pagamento); // Status: PagamentoConfirmado

        //    // Act
        //    pedido.MarcarComoEmSeparacao();

        //    // Assert
        //    pedido.StatusPedido.Should().Be(StatusPedido.EmSeparacao);
        //}

        //[Fact(DisplayName = "Não deve marcar como Em Separacao se não estiver PagamentoConfirmado")]
        //public void Nao_Deve_Marcar_Como_Em_Separacao_Se_Nao_Confirmado()
        //{
        //    // Arrange
        //    var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
        //    // Status: Pendente

        //    // Act
        //    Action act = () => pedido.MarcarComoEmSeparacao();

        //    // Assert
        //    act.Should().Throw<DomainException>()
        //        .WithMessage("O pedido só pode ir para 'Em Separação' após o pagamento ser confirmado.");
        //}

        [Fact(DisplayName = "Deve marcar pedido como Enviado")]
        public void Deve_Marcar_Como_Enviado()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            SetStatusPedido(pedido, StatusPedido.EmSeparacao);

            // Act
            pedido.MarcarComoEnviado();

            // Assert
            pedido.StatusPedido.Should().Be(StatusPedido.Enviado);
        }

        [Fact(DisplayName = "Não deve marcar como Enviado se não estiver Em Separacao")]
        public void Nao_Deve_Marcar_Como_Enviado_Se_Nao_EmSeparacao()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            SetStatusPedido(pedido, StatusPedido.PagamentoConfirmado);

            // Act
            Action act = () => pedido.MarcarComoEnviado();

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("O pedido só pode ser 'Enviado' após estar 'Em Separação'.");
        }

        [Fact(DisplayName = "Deve marcar pedido como Entregue")]
        public void Deve_Marcar_Como_Entregue()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            SetStatusPedido(pedido, StatusPedido.Enviado);

            // Act
            pedido.MarcarComoEntregue();

            // Assert
            pedido.StatusPedido.Should().Be(StatusPedido.Entregue);
        }

        [Fact(DisplayName = "Não deve marcar como Entregue se não estiver Enviado")]
        public void Nao_Deve_Marcar_Como_Entregue_Se_Nao_Enviado()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            SetStatusPedido(pedido, StatusPedido.EmSeparacao);

            // Act
            Action act = () => pedido.MarcarComoEntregue();

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("O pedido só pode ser marcado como 'Entregue' após ser 'Enviado'.");
        }

        [Fact(DisplayName = "Deve cancelar pedido Pendente")]
        public void Deve_Cancelar_Pedido_Pendente()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto", 50m, 1);

            // Act
            pedido.CancelarPedido();

            // Assert
            pedido.StatusPedido.Should().Be(StatusPedido.Cancelado);
        }

        [Fact(DisplayName = "Deve cancelar pedido PagamentoConfirmado")]
        public void Deve_Cancelar_Pedido_PagamentoConfirmado()
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto", 50m, 1);
            var pagamento = pedido.IniciarPagamento(MetodoPagamento.Pix);
            pedido.DefinirCodigoTransacao(pagamento);

            pedido.ConfirmarPagamento(pagamento); // Status: PagamentoConfirmado

            // Act  
            pedido.CancelarPedido();

            // Assert
            pedido.StatusPedido.Should().Be(StatusPedido.Cancelado);
        }

        [Theory(DisplayName = "Não deve permitir cancelar pedido após EmSeparacao")]
        [InlineData(StatusPedido.EmSeparacao)]
        [InlineData(StatusPedido.Enviado)]
        [InlineData(StatusPedido.Entregue)]
        public void Nao_Deve_Cancelar_Apos_EmSeparacao(StatusPedido status)
        {
            // Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            SetStatusPedido(pedido, status);

            // Act
            Action act = () => pedido.CancelarPedido();

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Não é possível cancelar um pedido que já está em separação ou posterior.");
        }
    }
}
