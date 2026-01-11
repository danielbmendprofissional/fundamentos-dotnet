using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Clientes.Enums;
using Vendas.Domain.Clientes.Events;
using Vendas.Domain.Clientes.Interfaces;
using Vendas.Domain.Clientes.ValueObjects;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Clientes.Entities
{
    public sealed class Cliente : AggregateRoot
    {
        public NomeCompleto Nome { get; private set; }
        public Cpf Cpf { get; private set; }
        public Email Email { get; private set; }
        public Telefone Telefone { get; private set; }
        public StatusCliente Status { get; private set; }
        public Sexo Sexo { get; private set; }
        public EstadoCivil EstadoCivil { get; private set; }
        public Guid EnderecoPrincipalId { get; private set; }

        private readonly List<Endereco> _enderecos = new();
        public IReadOnlyCollection<Endereco> Enderecos => _enderecos.ToList().AsReadOnly();

        private Cliente(
        NomeCompleto nome,
        Cpf cpf,
        Email email,
        Telefone telefone,
        Endereco enderecoPrincipal,
        Sexo sexo,
        EstadoCivil estadoCivil)
        {
            Validar(nome, cpf, email, telefone, enderecoPrincipal);

            Nome = nome;
            Cpf = cpf;
            Email = email;
            Telefone = telefone;
            Status = StatusCliente.Ativo;

            Sexo = sexo;
            EstadoCivil = estadoCivil;

            _enderecos.Add(enderecoPrincipal);
            EnderecoPrincipalId = enderecoPrincipal.Id;

            AddDomainEvent(new ClienteCadastradoEvent(
                ClienteId: Id,
                Nome: Nome.NomeCompletoFormatado,
                Cpf: Cpf.Numero,
                Email: Email.Endereco));
        }
        public static Cliente Criar(
            NomeCompleto nome, Cpf cpf, Email email, Telefone telefone, 
            DadosEndereco enderecoPrincipal, Sexo sexo = Sexo.NaoInformado,
            EstadoCivil estadoCivil = EstadoCivil.NaoInformado)
        {
            Guard.AgainstNull(nome, nameof(nome));
            Guard.AgainstNull(cpf, nameof(cpf));
            Guard.AgainstNull(email, nameof(email));
            Guard.AgainstNull(telefone, nameof(telefone));
            Guard.AgainstNull(enderecoPrincipal, nameof(enderecoPrincipal));
            Guard.AgainstInvalidEnum<Sexo>(sexo, nameof(sexo));
            Guard.AgainstInvalidEnum<EstadoCivil>(estadoCivil, nameof(estadoCivil));

            var endereco = new Endereco(
                enderecoPrincipal.Cep,
                enderecoPrincipal.Logradouro,
                enderecoPrincipal.Numero,
                enderecoPrincipal.Bairro,
                enderecoPrincipal.Cidade,
                enderecoPrincipal.Estado,
                enderecoPrincipal.Pais,
                enderecoPrincipal.Complemento);

            return new Cliente(
                nome,
                cpf,
                email,
                telefone,
                endereco,
                sexo,
                estadoCivil);
        }

        public void AdicionarEndereco(DadosEndereco dados)
        {
            Guard.AgainstNull(dados, nameof(dados));

            var endereco = new Endereco(
                dados.Cep,
                dados.Logradouro,
                dados.Numero,
                dados.Bairro,
                dados.Cidade,
                dados.Estado,
                dados.Pais,
                dados.Complemento);

            _enderecos.Add(endereco);
            SetDataAtualizacao();
        }


        public void RemoverEndereco(Guid enderecoId)
        {
            var endereco = _enderecos.FirstOrDefault(e => e.Id == enderecoId);
            Guard.AgainstNull(endereco, nameof(endereco));

            Guard.Against<DomainException>(
                _enderecos.Count == 1,
                "O cliente deve possuir ao menos um endereço.");

            _enderecos.Remove(endereco!);

            // Se removeu o principal, escolhe outro automaticamente
            if (enderecoId == EnderecoPrincipalId)
            {
                EnderecoPrincipalId = _enderecos.First().Id;

                AddDomainEvent(new EnderecoPrincipalAlteradoEvent(
                    ClienteId: Id,
                    NovoEnderecoId: EnderecoPrincipalId));
            }

            SetDataAtualizacao();
        }

        public void AlterarEndereco(
        Guid enderecoId,
        string cep,
        string logradouro,
        string numero,
        string bairro,
        string cidade,
        string estado,
        string pais,
        string? complemento = "")
        {
            var endereco = _enderecos.FirstOrDefault(e => e.Id == enderecoId);
            Guard.AgainstNull(endereco, nameof(endereco));

            endereco!.Atualizar(cep, logradouro, numero, bairro, cidade, estado, pais, complemento);

            SetDataAtualizacao();
        }

        public void DefinirEnderecoPrincipal(Guid enderecoId)
        {
            var endereco = _enderecos.FirstOrDefault(e => e.Id == enderecoId);
            Guard.AgainstNull(endereco, nameof(endereco));

            EnderecoPrincipalId = endereco!.Id;

            AddDomainEvent(new EnderecoPrincipalAlteradoEvent(
                ClienteId: Id,
                NovoEnderecoId: EnderecoPrincipalId));

            SetDataAtualizacao();
        }

        public void AtualizarPerfil(
        NomeCompleto nome,
        Email email,
        Telefone telefone,
        Sexo sexo,
        EstadoCivil estadoCivil)
        {
            Guard.Against<DomainException>(
                Status == StatusCliente.Bloqueado,
                "Clientes bloqueados não podem atualizar o perfil.");

            Guard.AgainstNull(nome, nameof(nome));
            Guard.AgainstNull(email, nameof(email));
            Guard.AgainstNull(telefone, nameof(telefone));

            Nome = nome;
            Email = email;
            Telefone = telefone;

            Sexo = sexo;
            EstadoCivil = estadoCivil;

            SetDataAtualizacao();
        }

        public void Bloquear()
        {
            if (Status == StatusCliente.Bloqueado)
                return;

            Status = StatusCliente.Bloqueado;

            AddDomainEvent(new ClienteBloqueadoEvent(
                ClienteId: Id,
                Cpf: Cpf.Numero));

            SetDataAtualizacao();
        }

        public void Ativar()
        {
            Status = StatusCliente.Ativo;
            SetDataAtualizacao();
        }

        //Nós validamos somente as invariantes do domínio, os enums são apenas complementares, não necessitam
        //de validação!
        private static void Validar(
        NomeCompleto nome,
        Cpf cpf,
        Email email,
        Telefone telefone,
        Endereco endereco)
        {
            Guard.AgainstNull(nome, nameof(nome));
            Guard.AgainstNull(cpf, nameof(cpf));
            Guard.AgainstNull(email, nameof(email));
            Guard.AgainstNull(telefone, nameof(telefone));
            Guard.AgainstNull(endereco, nameof(endereco));
        }
    }
}
