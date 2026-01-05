using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Domain.Tests.ValueObjects
{
    public class EnderecoEntregaTests
    {
        #region FACT -> SHOULD

        [Fact(DisplayName = "Should create DeliveryAdress with sucess when all data are valid")]
        public void Create_ShouldReturnValidAdress_WhenValidData()
        {
            //Arrange
            var cep = "40672249";
            var logradouro = "Rua das Flores";
            var complemento = "Apto 101";
            var bairro = "Centro";
            var estado = "SP";
            var cidade = "São Paulo";
            var pais = "Brasil";

            //Act
            var endereco = EnderecoEntrega.Criar(cep, logradouro, complemento, bairro, estado, cidade, pais);

            //Assert
            endereco.Should().NotBeNull();
            endereco.Cep.Should().Be(Cep.Criar(cep));
            endereco.Logradouro.Should().Be(logradouro);
            endereco.Complemento.Should().Be(complemento);
            endereco.Bairro.Should().Be(bairro);
            endereco.Estado.Should().Be(estado);
            endereco.Cidade.Should().Be(cidade);
            endereco.Pais.Should().Be(pais);
        }

        [Fact(DisplayName = "Adresses with the same values should be the same (VO)")]
        public void AdressShouldBeEqual_WhenTheyHaveTheSameValues()
        {
            //Arrange
            var endereco1 = EnderecoEntrega.Criar("12345678", "Rua x", "Casa", "Centro", "SP", "São Paulo", "Brasil");
            var endereco2 = EnderecoEntrega.Criar("12345678", "Rua x", "Casa", "Centro", "SP", "São Paulo", "Brasil");

            //Assert
            endereco1.Should().Be(endereco2);
            (endereco1 == endereco2).Should().BeTrue();
        }

        [Fact(DisplayName = "DeliveryAdress should be imutable after created")]
        public void AdressShouldBeImutable_AfterCreated()
        {
            //Arrange
            var endereco = EnderecoEntrega.Criar("12345678", "Rua x", "Casa", "Centro", "SP", "São Paulo", "Brasil");

            //Act
            Action act = () =>
            {
                //Hipotetic try (dont compile, only conceptual)
                //endereco.Cep = "999999999";
            };

            // Assert
            endereco.GetType().GetProperties()
                .All(p => p.SetMethod == null || p.SetMethod.IsPrivate)
                .Should().BeTrue("As propriedades do VO devem ser imutáveis");
        }

        #endregion

        #region FACT -> SHOULD NOT

        [Fact(DisplayName = "Adresses with the different values should be different (VO)")]
        public void AdressShouldDifferent_WhenTheyHaveDifferentValues()
        {
            //Arrange
            var endereco1 = EnderecoEntrega.Criar("12345678", "Rua y", "Casa", "Centro", "SP", "São Paulo", "Brasil");
            var endereco2 = EnderecoEntrega.Criar("12345678", "Rua x", "Casa", "Centro", "SP", "São Paulo", "Brasil");

            //Assert
            endereco1.Should().NotBe(endereco2);
            (endereco1 != endereco2).Should().BeTrue();
        }

        #endregion

        #region THEORY -> SHOULD

        [Theory(DisplayName = "Should throw Domain Exception when CEP is invalid")]
        [InlineData("1234567")]   //menos números
        [InlineData("123456789")] //mais números
        [InlineData("ABc12349")]  //carcteres inválidos
        public void Create_ShouldThrowDomainException_WhenCepIsInvalid(string cepInvalido)
        {
            //Arrange
            var logradouro = "Rua das Flores";
            var complemento = "Apto 101";
            var bairro = "Centro";
            var estado = "SP";
            var cidade = "São Paulo";
            var pais = "Brasil";

            //Act
            Action act = () => EnderecoEntrega.Criar(cepInvalido, logradouro, complemento, bairro, estado, cidade, pais);

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("CEP inválido. Deve conter 8 dígitos.");
        }

        [Theory(DisplayName = "Should throw Domain Exception when obrigated fields are null or empty")]
        [InlineData(null, "Rua y", "Centro", "SP", "São Paulo", "Brasil")]   //CEP null
        [InlineData("40672249", "", "Centro", "SP", "São Paulo", "Brasil")]  //Logradouro empty
        [InlineData("40672249", "Rua x", "Centro", "SP", " ", "Brasil")]     //City White space
        public void Create_ShouldThrowDomainException_WhenObrigatedFieldsNullOrEmpty(
            string cep, string logradouro,
            string bairro, string estado, string cidade, string pais)
        {

            //Act
            Action act = () => EnderecoEntrega.Criar(cep, logradouro, "complemento", bairro, estado, cidade, pais);

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("*Cannot be null or empty*");
        }

        #endregion
    }
}
