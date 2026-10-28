using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;
using Galo.Domain.Torcida.Enums;
using Galo.Domain.Torcida.Events;
using Galo.Domain.Torcida.ValueObjects;

namespace Galo.Domain.Torcida.Entities
{
    /// <summary>Aggregate Root do contexto Torcida (quem compra ingresso e é sócio).</summary>
    public sealed class Torcedor : AggregateRoot
    {
        public NomeCompleto Nome { get; private set; } = null!;
        public Cpf Cpf { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public Telefone? Telefone { get; private set; }
        public DateOnly DataNascimento { get; private set; }
        public PlanoSocio Plano { get; private set; }
        public StatusTorcedor Status { get; private set; }
        public Guid? EnderecoPrincipalId { get; private set; }

        private readonly List<Endereco> _enderecos = new();
        public IReadOnlyCollection<Endereco> Enderecos => _enderecos.AsReadOnly();

        private Torcedor() { } // EF Core

        private Torcedor(NomeCompleto nome, Cpf cpf, Email email, Telefone? telefone, DateOnly dataNascimento)
        {
            Guard.AgainstNull(nome, nameof(nome));
            Guard.AgainstNull(cpf, nameof(cpf));
            Guard.AgainstNull(email, nameof(email));
            Guard.Against<DomainException>(dataNascimento > DateOnly.FromDateTime(DateTime.UtcNow), "Data de nascimento inválida.");

            Nome = nome;
            Cpf = cpf;
            Email = email;
            Telefone = telefone;
            DataNascimento = dataNascimento;
            Plano = PlanoSocio.Nenhum;
            Status = StatusTorcedor.Ativo;

            AddDomainEvent(new TorcedorCadastradoEvent(Id, nome.NomeCompletoFormatado, email.Endereco));
        }

        public static Torcedor Cadastrar(NomeCompleto nome, Cpf cpf, Email email, Telefone? telefone, DateOnly dataNascimento)
            => new(nome, cpf, email, telefone, dataNascimento);

        public void AtualizarPerfil(NomeCompleto nome, Email email, Telefone? telefone)
        {
            Guard.AgainstNull(nome, nameof(nome));
            Guard.AgainstNull(email, nameof(email));
            Nome = nome;
            Email = email;
            Telefone = telefone;
            SetDataAtualizacao();
        }

        public void AlterarPlano(PlanoSocio novoPlano)
        {
            Guard.AgainstInvalidEnum(novoPlano, nameof(novoPlano));
            Guard.Against<DomainException>(Status == StatusTorcedor.Bloqueado, "Torcedor bloqueado não pode alterar o plano.");
            Guard.Against<DomainException>(Plano == novoPlano, "O torcedor já está neste plano.");

            var anterior = Plano;
            Plano = novoPlano;
            SetDataAtualizacao();

            AddDomainEvent(new PlanoSocioAlteradoEvent(Id, anterior, novoPlano));
        }

        public Guid AdicionarEndereco(string cep, string logradouro, string numero, string bairro,
            string cidade, string estado, string pais = "Brasil", string? complemento = null, bool principal = false)
        {
            var endereco = new Endereco(Id, cep, logradouro, numero, bairro, cidade, estado, pais, complemento);
            _enderecos.Add(endereco);

            // o primeiro endereço sempre vira o principal
            if (principal || EnderecoPrincipalId is null)
                DefinirEnderecoPrincipal(endereco.Id);

            SetDataAtualizacao();
            return endereco.Id;
        }

        public void DefinirEnderecoPrincipal(Guid enderecoId)
        {
            Guard.Against<DomainException>(_enderecos.All(e => e.Id != enderecoId), "Endereço não pertence ao torcedor.");

            EnderecoPrincipalId = enderecoId;
            SetDataAtualizacao();
            AddDomainEvent(new EnderecoPrincipalAlteradoEvent(Id, enderecoId));
        }

        public void RemoverEndereco(Guid enderecoId)
        {
            var endereco = _enderecos.FirstOrDefault(e => e.Id == enderecoId);
            Guard.Against<DomainException>(endereco is null, "Endereço não pertence ao torcedor.");
            Guard.Against<DomainException>(EnderecoPrincipalId == enderecoId,
                "Não é possível remover o endereço principal. Defina outro antes.");

            _enderecos.Remove(endereco!);
            SetDataAtualizacao();
        }

        public void Bloquear(string motivo)
        {
            Guard.AgainstNullOrWhiteSpace(motivo, nameof(motivo));
            Guard.Against<DomainException>(Status == StatusTorcedor.Bloqueado, "O torcedor já está bloqueado.");

            Status = StatusTorcedor.Bloqueado;
            SetDataAtualizacao();
            AddDomainEvent(new TorcedorBloqueadoEvent(Id, motivo.Trim()));
        }

        public void Ativar()
        {
            Guard.Against<DomainException>(Status == StatusTorcedor.Ativo, "O torcedor já está ativo.");
            Status = StatusTorcedor.Ativo;
            SetDataAtualizacao();
        }
    }
}
