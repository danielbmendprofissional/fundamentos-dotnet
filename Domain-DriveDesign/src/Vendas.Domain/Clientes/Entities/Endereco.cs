using System.Text.RegularExpressions;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Clientes.Entities
{
    public sealed class Endereco : Entity
    {
        public string Cep { get; private set; }
        public string Logradouro { get; private set; }
        public string Numero { get; private set; }
        public string Bairro { get; private set; }
        public string Cidade { get; private set; }
        public string Estado { get; private set; }
        public string Pais { get; private set; }
        public string Complemento { get; private set; }

        public Endereco(
        string cep,
        string logradouro,
        string numero,
        string bairro,
        string cidade,
        string estado,
        string pais,
        string complemento = "")
        {
            Validar(cep, logradouro, numero, bairro, cidade, estado, pais);

            Cep = cep;
            Logradouro = logradouro;
            Numero = numero;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            Pais = pais;
            Complemento = complemento;
        }

        internal void Atualizar(
        string cep,
        string logradouro,
        string numero,
        string bairro,
        string cidade,
        string estado,
        string pais,
        string complemento = "")
        {
            Validar(cep, logradouro, numero, bairro, cidade, estado, pais);

            Cep = cep;
            Logradouro = logradouro;
            Numero = numero;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            Pais = pais;
            Complemento = complemento;
        }

        private static void Validar(
        string cep,
        string logradouro,
        string numero,
        string bairro,
        string cidade,
        string estado,
        string pais)
        {
            Guard.AgainstNullOrWhiteSpace(cep, nameof(cep));
            Guard.Against<DomainException>(!Regex.IsMatch(cep, @"^\d{8}$"), "CEP inválido.");
            Guard.AgainstNullOrWhiteSpace(logradouro, nameof(logradouro));
            Guard.Against<DomainException>(logradouro.Length < 3, "Logradouro muito curto.");
            Guard.AgainstNullOrWhiteSpace(numero, nameof(numero));
            Guard.Against<DomainException>(numero.Length == 0, "Número inválido.");
            Guard.AgainstNullOrWhiteSpace(bairro, nameof(bairro));
            Guard.AgainstNullOrWhiteSpace(cidade, nameof(cidade));
            Guard.AgainstNullOrWhiteSpace(estado, nameof(estado));
            Guard.AgainstNullOrWhiteSpace(pais, nameof(pais));
        }

    }

}
