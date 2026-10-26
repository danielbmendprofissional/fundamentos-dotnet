using FluentAssertions;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Ingressos.Entities;
using Galo.Domain.Ingressos.Enums;
using Galo.Domain.Ingressos.Events;
using Galo.Domain.Ingressos.Services;
using Galo.Domain.Ingressos.ValueObjects;
using Galo.Domain.Torcida.Enums;

namespace Galo.Domain.Tests.Ingressos
{
    public class CompraIngressoTests
    {
        private static CompraIngresso NovaCompra()
        {
            var c = CompraIngresso.Criar(Guid.NewGuid(), Guid.NewGuid());
            c.AdicionarItem("Norte", 80m, 2);
            return c;
        }

        /// <summary>Leva a compra até "reserva confirmada" (o que o consumidor RabbitMQ faria).</summary>
        private static (CompraIngresso Compra, Guid PagamentoId) AteReservaConfirmada()
        {
            var c = NovaCompra();
            var pagamentoId = c.IniciarPagamento(MetodoPagamento.Pix);
            c.ConfirmarReserva();
            c.ClearDomainEvents();
            return (c, pagamentoId);
        }

        [Fact(DisplayName = "Compra nova é pendente, com número gerado e total zerado")]
        public void Compra_Nova()
        {
            var c = CompraIngresso.Criar(Guid.NewGuid(), Guid.NewGuid());

            c.Status.Should().Be(StatusCompra.Pendente);
            c.ValorTotal.Should().Be(0);
            c.Numero.Should().StartWith("ING-");
            c.ReservaConfirmada.Should().BeFalse();
        }

        [Fact(DisplayName = "Adicionar o mesmo setor soma as unidades em vez de duplicar o item")]
        public void Soma_Unidades_Do_Mesmo_Setor()
        {
            var c = NovaCompra();
            c.AdicionarItem("norte", 80m, 3);
            c.AdicionarItem("Sul", 80m, 1);

            c.Itens.Should().HaveCount(2);
            c.Itens.Single(i => i.Setor == "Norte").Quantidade.Should().Be(5);
            c.ValorTotal.Should().Be(480m);
            c.TotalDeIngressos.Should().Be(6);
        }

        [Fact(DisplayName = "Desconto de sócio reduz o valor total")]
        public void Desconto_De_Socio()
        {
            var c = NovaCompra();                 // 2 x 80 = 160
            c.AplicarDesconto(25m);

            c.ValorTotal.Should().Be(120m);
            c.Itens.Single().PorcentagemDesconto.Should().Be(25m);
        }

        [Theory(DisplayName = "Domain service: percentual de desconto por plano")]
        [InlineData(PlanoSocio.Nenhum, 0)]
        [InlineData(PlanoSocio.Bronze, 5)]
        [InlineData(PlanoSocio.Prata, 10)]
        [InlineData(PlanoSocio.Ouro, 20)]
        [InlineData(PlanoSocio.Diamante, 30)]
        public void Desconto_Por_Plano(PlanoSocio plano, decimal esperado)
            => new CalculadoraDeDescontoService().PercentualParaPlano(plano).Should().Be(esperado);

        [Fact(DisplayName = "Não inicia pagamento de compra sem itens")]
        public void Pagamento_Sem_Itens()
        {
            var c = CompraIngresso.Criar(Guid.NewGuid(), Guid.NewGuid());
            var act = () => c.IniciarPagamento(MetodoPagamento.Pix);
            act.Should().Throw<DomainException>().WithMessage("*sem itens*");
        }

        [Fact(DisplayName = "Iniciar pagamento: aguarda pagamento e emite evento com os itens (snapshot)")]
        public void Iniciar_Pagamento()
        {
            var c = NovaCompra();

            var pagamentoId = c.IniciarPagamento(MetodoPagamento.CartaoCredito);

            c.Status.Should().Be(StatusCompra.AguardandoPagamento);
            c.Pagamentos.Should().ContainSingle(p => p.Id == pagamentoId && p.Status == StatusPagamento.Pendente && p.Valor == 160m);
            c.DomainEvents.OfType<PagamentoIniciadoEvent>().Should().ContainSingle()
                .Which.Itens.Should().ContainSingle(i => i.Setor == "Norte" && i.Quantidade == 2);
        }

        [Fact(DisplayName = "Com pagamento iniciado o carrinho não pode mais ser alterado")]
        public void Carrinho_Congelado()
        {
            var (c, _) = AteReservaConfirmada();
            var act = () => c.AdicionarItem("Sul", 80m, 1);
            act.Should().Throw<DomainException>().WithMessage("*pendente*");
        }

        [Fact(DisplayName = "Pagamento só pode ser confirmado DEPOIS da reserva dos ingressos")]
        public void Confirmar_Pagamento_Exige_Reserva()
        {
            var c = NovaCompra();
            var pagamentoId = c.IniciarPagamento(MetodoPagamento.Pix);

            var act = () => c.ConfirmarPagamento(pagamentoId);

            act.Should().Throw<DomainException>().WithMessage("*reservados*");
        }

        [Fact(DisplayName = "Fluxo feliz: reserva -> pagamento confirmado -> ingressos emitidos")]
        public void Fluxo_Feliz()
        {
            var (c, pagamentoId) = AteReservaConfirmada();

            c.ConfirmarPagamento(pagamentoId);
            c.Status.Should().Be(StatusCompra.PagamentoConfirmado);
            c.Pagamentos.Single().Status.Should().Be(StatusPagamento.Aprovado);
            c.DomainEvents.OfType<PagamentoConfirmadoEvent>().Should().ContainSingle();

            var codigos = c.EmitirIngressos();

            c.Status.Should().Be(StatusCompra.IngressosEmitidos);
            codigos.Should().HaveCount(2).And.OnlyHaveUniqueItems();
            codigos.Should().OnlyContain(codigo => codigo.StartsWith(c.Numero));
            c.DomainEvents.OfType<IngressosEmitidosEvent>().Should().ContainSingle();
        }

        [Fact(DisplayName = "Não emite ingressos antes de confirmar o pagamento")]
        public void Emitir_Exige_Pagamento()
        {
            var (c, _) = AteReservaConfirmada();
            var act = () => c.EmitirIngressos();
            act.Should().Throw<DomainException>().WithMessage("*confirmação do pagamento*");
        }

        [Fact(DisplayName = "Pagamento recusado devolve a compra para pendente (pode tentar de novo)")]
        public void Pagamento_Recusado_Volta_Para_Pendente()
        {
            var (c, pagamentoId) = AteReservaConfirmada();

            c.RecusarPagamento(pagamentoId);

            c.Status.Should().Be(StatusCompra.Pendente);
            c.Pagamentos.Single().Status.Should().Be(StatusPagamento.Recusado);
            c.DomainEvents.OfType<PagamentoRecusadoEvent>().Should().ContainSingle();
        }

        [Fact(DisplayName = "Cancelar sem reserva NÃO pede devolução de estoque; com reserva SIM")]
        public void Cancelar_Informa_Se_Havia_Reserva()
        {
            var semReserva = NovaCompra();
            semReserva.IniciarPagamento(MetodoPagamento.Pix);
            semReserva.ClearDomainEvents();
            semReserva.Cancelar(MotivoCancelamento.IngressosEsgotados());
            semReserva.DomainEvents.OfType<CompraCanceladaEvent>().Single().ReservaConfirmada.Should().BeFalse();

            var (comReserva, _) = AteReservaConfirmada();
            comReserva.Cancelar(MotivoCancelamento.TorcedorDesistiu());
            var evento = comReserva.DomainEvents.OfType<CompraCanceladaEvent>().Single();
            evento.ReservaConfirmada.Should().BeTrue();
            evento.Itens.Should().ContainSingle(i => i.Setor == "Norte" && i.Quantidade == 2);
            comReserva.MotivoCancelamento.Should().Be(MotivoCancelamento.TorcedorDesistiu());
        }

        [Fact(DisplayName = "Não cancela compra com ingressos já emitidos nem cancela duas vezes")]
        public void Cancelamento_Invalido()
        {
            var (c, pagamentoId) = AteReservaConfirmada();
            c.ConfirmarPagamento(pagamentoId);
            c.EmitirIngressos();

            var depoisDeEmitir = () => c.Cancelar();
            depoisDeEmitir.Should().Throw<DomainException>().WithMessage("*emitidos*");

            var outra = NovaCompra();
            outra.Cancelar();
            var duasVezes = () => outra.Cancelar();
            duasVezes.Should().Throw<DomainException>().WithMessage("*já está cancelada*");
        }

        [Fact(DisplayName = "Motivo de cancelamento só aceita códigos do domínio")]
        public void Motivo_Invalido()
        {
            var act = () => MotivoCancelamento.De("Inventado");
            act.Should().Throw<DomainException>();
        }
    }
}
