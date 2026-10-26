using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;
using Galo.Domain.Elenco.Enums;
using Galo.Domain.Elenco.Events;
using Galo.Domain.Elenco.ValueObjects;

namespace Galo.Domain.Elenco.Entities
{
    /// <summary>Aggregate Root do contexto Elenco. Contratos são entidades filhas.</summary>
    public sealed class Jogador : AggregateRoot
    {
        public NomeJogador Nome { get; private set; } = null!;
        public string? Apelido { get; private set; }
        public NumeroCamisa Camisa { get; private set; } = null!;
        public Posicao Posicao { get; private set; }
        public DateOnly DataNascimento { get; private set; }
        public string Nacionalidade { get; private set; } = string.Empty;
        public ValorMercado ValorMercado { get; private set; } = null!;
        public StatusJogador Status { get; private set; }

        private readonly List<Contrato> _contratos = new();
        public IReadOnlyCollection<Contrato> Contratos => _contratos.AsReadOnly();

        private Jogador() { } // EF Core

        private Jogador(NomeJogador nome, string? apelido, NumeroCamisa camisa, Posicao posicao,
            DateOnly dataNascimento, string nacionalidade, ValorMercado valorMercado)
        {
            Guard.AgainstNull(nome, nameof(nome));
            Guard.AgainstNull(camisa, nameof(camisa));
            Guard.AgainstNull(valorMercado, nameof(valorMercado));
            Guard.AgainstInvalidEnum(posicao, nameof(posicao));
            Guard.AgainstNullOrWhiteSpace(nacionalidade, nameof(nacionalidade));
            Guard.Against<DomainException>(dataNascimento > DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-14)),
                "O jogador deve ter ao menos 14 anos.");

            Nome = nome;
            Apelido = apelido?.Trim();
            Camisa = camisa;
            Posicao = posicao;
            DataNascimento = dataNascimento;
            Nacionalidade = nacionalidade.Trim();
            ValorMercado = valorMercado;
            Status = StatusJogador.Ativo;

            AddDomainEvent(new JogadorCadastradoEvent(Id, nome.Valor));
        }

        public static Jogador Criar(NomeJogador nome, string? apelido, NumeroCamisa camisa, Posicao posicao,
            DateOnly dataNascimento, string nacionalidade, ValorMercado valorMercado)
            => new(nome, apelido, camisa, posicao, dataNascimento, nacionalidade, valorMercado);

        public int CalcularIdade(DateOnly hoje)
        {
            var idade = hoje.Year - DataNascimento.Year;
            if (hoje < DataNascimento.AddYears(idade)) idade--;
            return idade;
        }

        public void AlterarCamisa(NumeroCamisa novaCamisa)
        {
            Guard.AgainstNull(novaCamisa, nameof(novaCamisa));
            Camisa = novaCamisa;
            SetDataAtualizacao();
        }

        public void AtualizarValorMercado(ValorMercado novoValor)
        {
            Guard.AgainstNull(novoValor, nameof(novoValor));

            var anterior = ValorMercado.Valor;
            ValorMercado = novoValor;
            SetDataAtualizacao();

            AddDomainEvent(new ValorMercadoAtualizadoEvent(Id, anterior, novoValor.Valor));
        }

        public Guid RegistrarContrato(DateOnly inicio, DateOnly fim, decimal salarioMensal, decimal? clausulaRescisoria = null)
        {
            Guard.Against<DomainException>(Status is StatusJogador.Transferido or StatusJogador.Aposentado,
                "Não é possível registrar contrato para jogador transferido ou aposentado.");
            Guard.Against<DomainException>(_contratos.Any(c => c.SobrepoeA(inicio, fim)),
                "O período do contrato se sobrepõe a um contrato existente.");

            var contrato = new Contrato(Id, inicio, fim, salarioMensal, clausulaRescisoria);
            _contratos.Add(contrato);
            SetDataAtualizacao();

            AddDomainEvent(new ContratoRegistradoEvent(Id, contrato.Id, inicio, fim));
            return contrato.Id;
        }

        public Contrato? ContratoVigente(DateOnly hoje) => _contratos.FirstOrDefault(c => c.EstaVigente(hoje));

        public void Lesionar(string descricao)
        {
            Guard.AgainstNullOrWhiteSpace(descricao, nameof(descricao));
            Guard.Against<DomainException>(Status != StatusJogador.Ativo, "Apenas jogadores ativos podem ser lesionados.");

            Status = StatusJogador.Lesionado;
            SetDataAtualizacao();
            AddDomainEvent(new JogadorLesionadoEvent(Id, descricao.Trim()));
        }

        public void Recuperar()
        {
            Guard.Against<DomainException>(Status != StatusJogador.Lesionado, "O jogador não está lesionado.");

            Status = StatusJogador.Ativo;
            SetDataAtualizacao();
            AddDomainEvent(new JogadorRecuperadoEvent(Id));
        }

        public void Emprestar()
        {
            Guard.Against<DomainException>(Status != StatusJogador.Ativo, "Apenas jogadores ativos podem ser emprestados.");
            Status = StatusJogador.Emprestado;
            SetDataAtualizacao();
        }

        public void Transferir()
        {
            Guard.Against<DomainException>(Status == StatusJogador.Transferido, "O jogador já foi transferido.");
            Status = StatusJogador.Transferido;
            SetDataAtualizacao();
        }
    }
}
