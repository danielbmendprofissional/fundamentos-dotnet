using FluentAssertions;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Competicoes.ValueObjects;
using Galo.Domain.Elenco.ValueObjects;
using Galo.Domain.Torcida.ValueObjects;

namespace Galo.Domain.Tests.Torcida
{
    public class ValueObjectsTests
    {
        // ---------------- CPF ----------------
        [Theory(DisplayName = "CPF válido deve ser aceito e normalizado")]
        [InlineData("529.982.247-25")]
        [InlineData("52998224725")]
        public void Cpf_Valido(string entrada)
        {
            var cpf = Cpf.Create(entrada);
            cpf.Numero.Should().Be("52998224725");
            cpf.ToString().Should().Be("529.982.247-25");
        }

        [Theory(DisplayName = "CPF inválido deve ser recusado")]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("11111111111")]   // sequência repetida
        [InlineData("52998224726")]   // dígito verificador errado
        public void Cpf_Invalido(string entrada)
        {
            var act = () => Cpf.Create(entrada);
            act.Should().Throw<DomainException>();
        }

        [Fact(DisplayName = "CPFs gerados para o seed devem ser sempre válidos")]
        public void Cpf_GerarValido_Sempre_Passa_Na_Validacao()
        {
            for (long i = 1; i <= 500; i++)
            {
                var gerado = Cpf.GerarValido(100_000_000L + i);
                var act = () => Cpf.Create(gerado);
                act.Should().NotThrow($"o CPF gerado {gerado} deveria ser válido");
            }
        }

        // ---------------- E-mail / telefone / nome ----------------
        [Fact(DisplayName = "E-mail deve ser normalizado em minúsculas e comparado por valor")]
        public void Email_Normaliza_E_Compara_Por_Valor()
        {
            var a = Email.Create("  Galo@Atletico.COM.br ");
            var b = Email.Create("galo@atletico.com.br");

            a.Endereco.Should().Be("galo@atletico.com.br");
            a.Should().Be(b);
            (a == b).Should().BeTrue();
        }

        [Theory(DisplayName = "E-mail inválido deve ser recusado")]
        [InlineData("sem-arroba")]
        [InlineData("a@b")]
        [InlineData("")]
        public void Email_Invalido(string entrada)
        {
            var act = () => Email.Create(entrada);
            act.Should().Throw<DomainException>();
        }

        [Theory(DisplayName = "Telefone deve formatar fixo e celular")]
        [InlineData("31 99999-1234", "(31) 99999-1234")]
        [InlineData("(31) 3333-1234", "(31) 3333-1234")]
        public void Telefone_Formata(string entrada, string esperado)
            => Telefone.Create(entrada).ToString().Should().Be(esperado);

        [Fact(DisplayName = "Nome completo exige ao menos nome e sobrenome")]
        public void NomeCompleto_Exige_Sobrenome()
        {
            var act = () => NomeCompleto.Create("Madonna");
            act.Should().Throw<DomainException>();

            var nome = NomeCompleto.Create("  Daniel   Bastos  Mendes ");
            nome.Nome.Should().Be("Daniel Bastos");
            nome.Sobrenome.Should().Be("Mendes");
            nome.NomeResumido.Should().Be("Daniel Mendes");
        }

        // ---------------- Elenco ----------------
        [Theory(DisplayName = "Camisa deve estar entre 1 e 99")]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(-5)]
        public void Camisa_Invalida(int numero)
        {
            var act = () => NumeroCamisa.Create(numero);
            act.Should().Throw<DomainException>();
        }

        [Fact(DisplayName = "Valor de mercado não pode ser negativo")]
        public void ValorMercado_Negativo()
        {
            var act = () => ValorMercado.Create(-1m);
            act.Should().Throw<DomainException>();
            ValorMercado.Create(0m).Valor.Should().Be(0m);
        }

        // ---------------- Placar ----------------
        [Fact(DisplayName = "Placar é imutável: marcar gol devolve um NOVO placar")]
        public void Placar_E_Imutavel()
        {
            var zerado = Placar.Zerado();
            var umAZero = zerado.GolMandante();

            zerado.ToString().Should().Be("0 x 0");
            umAZero.ToString().Should().Be("1 x 0");
            umAZero.GolVisitante().Should().Be(Placar.Create(1, 1));
        }
    }
}
