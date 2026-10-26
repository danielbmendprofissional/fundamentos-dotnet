using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Clientes.Entities;
using Vendas.Domain.Clientes.Enums;
using Vendas.Domain.Clientes.Events;
using Vendas.Domain.Clientes.Interfaces;
using Vendas.Domain.Clientes.ValueObjects;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Domain.Tests.Clientes.Entities
{
    public class ClientTests
    {
        private static NomeCompleto CriarNomeCompleto(string nome = "João Silva")
            => NomeCompleto.Create(nome);

        private static Cpf CriarCpf(string cpf = "12345678909")
            => Cpf.Create(cpf);

        private static Email CriarEmail(string email = "joao@example.com")
            => Email.Create(email);

        private static Telefone CriarTelefone(string telefone = "11999999999")
            => Telefone.Create(telefone);

        private static DadosEndereco CriarDadosEndereco(
            string cep = "01310100",
            string logradouro = "Avenida Paulista",
            string numero = "1000",
            string bairro = "Bela Vista",
            string cidade = "São Paulo",
            string estado = "SP",
            string pais = "Brasil",
            string complemento = "")
        {
            return DadosEndereco.Criar(
                cep, logradouro, numero, bairro, cidade, estado, pais, complemento);
        }



        private static Cliente CriarClienteValido()
            => Cliente.Criar(
                CriarNomeCompleto(),
                CriarCpf(),
                CriarEmail(),
                CriarTelefone(),
                CriarDadosEndereco(),
                Sexo.Masculino,
                EstadoCivil.Solteiro);

        [Fact]
        public void Constructor_ComDadosValidos_DeveCriarCliente()
        {
            var cliente = CriarClienteValido();

            cliente.Status.Should().Be(StatusCliente.Ativo);
            cliente.Sexo.Should().Be(Sexo.Masculino);
            cliente.EstadoCivil.Should().Be(EstadoCivil.Solteiro);

            cliente.Enderecos.Should().ContainSingle();
            cliente.EnderecoPrincipalId.Should().Be(cliente.Enderecos.First().Id);
        }

        [Fact]
        public void Constructor_DeveGerarEventoClienteCadastrado()
        {
            var cliente = CriarClienteValido();

            cliente.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<ClienteCadastradoEvent>();
        }

        [Theory]
        [InlineData("Nome")]
        [InlineData("Cpf")]
        [InlineData("Email")]
        [InlineData("Telefone")]
        [InlineData("Endereco")]
        public void Constructor_ComParametroObrigatorioNulo_DeveLancarDomainException(string campo)
        {
            NomeCompleto? nome = campo == "Nome" ? null : CriarNomeCompleto();
            Cpf? cpf = campo == "Cpf" ? null : CriarCpf();
            Email? email = campo == "Email" ? null : CriarEmail();
            Telefone? telefone = campo == "Telefone" ? null : CriarTelefone();
            DadosEndereco? endereco = campo == "Endereco" ? null : CriarDadosEndereco();

            Action act = () => Cliente.Criar(nome!, cpf!, email!, telefone!, endereco!);

            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void AdicionarEndereco_DeveAdicionar()
        {
            var cliente = CriarClienteValido();
            var novo = CriarDadosEndereco();

            cliente.AdicionarEndereco(novo);

            cliente.Enderecos.Should().HaveCount(2);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AdicionarEndereco_ValidacaoDeNulo(bool usarNulo)
        {
            var cliente = CriarClienteValido();
            DadosEndereco? endereco = usarNulo ? null : CriarDadosEndereco();

            Action act = () => cliente.AdicionarEndereco(endereco!);

            if (usarNulo)
                act.Should().Throw<DomainException>();
            else
                act.Should().NotThrow();
        }

        [Fact]
        public void AdicionarEndereco_DeveAtualizarDataModificacao()
        {
            var cliente = CriarClienteValido();
            var dataAnterior = cliente.DataAtualizacao ?? DateTime.UtcNow;

            Thread.Sleep(5);

            cliente.AdicionarEndereco(DadosEndereco.Criar(
                cep: "02134000",
                logradouro: "Rua Augusta",
                numero: "100",
                bairro: "Consolação",
                cidade: "São Paulo",
                estado: "SP",
                pais: "Brasil",
                complemento: ""
            ));

            cliente.DataAtualizacao.Should().BeAfter(dataAnterior);
        }


        [Fact]
        public void RemoverEndereco_ComSegundoEndereco_DeveRemover()
        {
            var cliente = CriarClienteValido();

            cliente.AdicionarEndereco(CriarDadosEndereco(
                cep: "02134000",
                logradouro: "Rua Augusta"
            ));

            var segundoEnderecoId = cliente.Enderecos.Last().Id;

            cliente.RemoverEndereco(segundoEnderecoId);

            cliente.Enderecos.Should().HaveCount(1);
        }


        [Theory]
        [InlineData("NaoExiste")]
        [InlineData("Ultimo")]
        public void RemoverEndereco_DeveLancarExceptions(string caso)
        {
            var cliente = CriarClienteValido();
            Guid id = caso switch
            {
                "NaoExiste" => Guid.NewGuid(),
                "Ultimo" => cliente.EnderecoPrincipalId,
                _ => throw new ArgumentOutOfRangeException()
            };

            Action act = () => cliente.RemoverEndereco(id);

            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void RemoverEndereco_Principal_DeveRedefinirPrincipal()
        {
            var cliente = CriarClienteValido();

            var dadosSegundo = CriarDadosEndereco("02134000", "Rua Augusta");
            cliente.AdicionarEndereco(dadosSegundo);

            var segundoEnderecoId = cliente.Enderecos.Last().Id;

            cliente.RemoverEndereco(cliente.EnderecoPrincipalId);

            cliente.EnderecoPrincipalId.Should().Be(segundoEnderecoId);
            cliente.DomainEvents.Should().Contain(e => e is EnderecoPrincipalAlteradoEvent);
        }


        //[Theory]
        //[InlineData(true)]
        //[InlineData(false)]
        //public void AlterarEndereco_ValidacaoDeEndereco(bool valido)
        //{
        //    var cliente = CriarClienteValido();
        //    var principal = cliente.ObterEnderecoPrincipal();

        //    Guid id = valido ? principal.Id : Guid.NewGuid();

        //    Action act = () => cliente.AlterarEndereco(
        //        id, "02134000", "Rua Nova", "1", "Centro", "São Paulo", "SP", "Brasil");

        //    if (valido)
        //        act.Should().NotThrow();
        //    else
        //        act.Should().Throw<DomainException>();
        //}

        //[Fact]
        //public void AlterarEndereco_DeveAlterarCampos()
        //{
        //    var cliente = CriarClienteValido();
        //    var principal = cliente.ObterEnderecoPrincipal();

        //    cliente.AlterarEndereco(
        //        principal.Id, "02134000", "Rua Nova", "1", "Centro", "São Paulo", "SP", "Brasil");

        //    principal.Logradouro.Should().Be("Rua Nova");
        //}

        [Fact]
        public void DefinirEnderecoPrincipal_DeveDefinir()
        {
            var cliente = CriarClienteValido();
            var novo = CriarDadosEndereco("02134000", "Rua Augusta");
            cliente.AdicionarEndereco(novo);

            var novoEnderecoId = cliente.Enderecos.Last().Id;

            cliente.DefinirEnderecoPrincipal(novoEnderecoId);

            cliente.EnderecoPrincipalId.Should().Be(novoEnderecoId);
        }

        [Fact]
        public void DefinirEnderecoPrincipal_DeveGerarEvento()
        {
            var cliente = CriarClienteValido();
            var novo = CriarDadosEndereco("02134000", "Rua Augusta");
            cliente.AdicionarEndereco(novo);

            var novoEnderecoId = cliente.Enderecos.Last().Id;

            cliente.DefinirEnderecoPrincipal(novoEnderecoId);

            cliente.DomainEvents.Should().Contain(e => e is EnderecoPrincipalAlteradoEvent);
        }

        //[Fact]
        //public void ObterEnderecoPrincipal_DeveRetornarCorreto()
        //{
        //    var cliente = CriarClienteValido();

        //    var principal = cliente.ObterEnderecoPrincipal();

        //    principal.Id.Should().Be(cliente.EnderecoPrincipalId);
        //}

        [Fact]
        public void AtualizarPerfil_DeveAtualizar()
        {
            var cliente = CriarClienteValido();
            var nome = CriarNomeCompleto("Maria Silva");

            cliente.AtualizarPerfil(
                nome,
                CriarEmail("maria@dex.com"),
                CriarTelefone("11988887777"),
                Sexo.Feminino,
                EstadoCivil.Casado);

            cliente.Nome.Should().Be(nome);
            cliente.Sexo.Should().Be(Sexo.Feminino);
            cliente.EstadoCivil.Should().Be(EstadoCivil.Casado);
        }

        [Theory]
        [InlineData("Nome")]
        [InlineData("Email")]
        [InlineData("Telefone")]
        public void AtualizarPerfil_CamposNulos_DeveFalhar(string campo)
        {
            var cliente = CriarClienteValido();

            NomeCompleto? nome = campo == "Nome" ? null : CriarNomeCompleto();
            Email? email = campo == "Email" ? null : CriarEmail();
            Telefone? telefone = campo == "Telefone" ? null : CriarTelefone();

            Action act = () => cliente.AtualizarPerfil(
                nome!, email!, telefone!,
                Sexo.Masculino,
                EstadoCivil.Solteiro);

            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void AtualizarPerfil_ComClienteBloqueado_DeveFalhar()
        {
            var cliente = CriarClienteValido();
            cliente.Bloquear();

            Action act = () => cliente.AtualizarPerfil(
                CriarNomeCompleto(),
                CriarEmail(),
                CriarTelefone(),
                Sexo.Masculino,
                EstadoCivil.Casado);

            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Bloquear_DeveBloquear()
        {
            var cliente = CriarClienteValido();

            cliente.Bloquear();

            cliente.Status.Should().Be(StatusCliente.Bloqueado);
        }

        [Fact]
        public void Bloquear_DeveGerarEvento()
        {
            var cliente = CriarClienteValido();

            cliente.Bloquear();

            cliente.DomainEvents.Should()
                .Contain(e => e is ClienteBloqueadoEvent);
        }

        [Fact]
        public void Ativar_DeveAtivar()
        {
            var cliente = CriarClienteValido();
            cliente.Bloquear();

            cliente.Ativar();

            cliente.Status.Should().Be(StatusCliente.Ativo);
        }

        [Fact]
        public void Fluxo_Completo_DeveManterConsistencia()
        {
            var cliente = CriarClienteValido();

            var e1 = CriarDadosEndereco("02134000", "Rua Augusta");
            cliente.AdicionarEndereco(e1);

            var e1Id = cliente.Enderecos.Last().Id;

            cliente.DefinirEnderecoPrincipal(e1Id);

            cliente.EnderecoPrincipalId.Should().Be(e1Id);
            cliente.Enderecos.Should().HaveCount(2);
        }

    }
}
