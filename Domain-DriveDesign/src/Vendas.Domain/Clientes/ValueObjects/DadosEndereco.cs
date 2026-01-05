using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Clientes.ValueObjects
{
    public sealed record DadosEndereco
    {
        public string Cep { get; }
        public string Logradouro { get; }
        public string Numero { get; }
        public string Bairro { get; }
        public string Cidade { get; }
        public string Estado { get; }
        public string Pais { get; }
        public string Complemento { get; }

        public DadosEndereco(
            string cep,
            string logradouro,
            string numero,
            string bairro,
            string cidade,
            string estado,
            string pais,
            string complemento = "")
        {
            Guard.AgainstNullOrWhiteSpace(cep, nameof(cep));
            Guard.AgainstNullOrWhiteSpace(logradouro, nameof(logradouro));
            Guard.AgainstNullOrWhiteSpace(numero, nameof(numero));
            Guard.AgainstNullOrWhiteSpace(bairro, nameof(bairro));
            Guard.AgainstNullOrWhiteSpace(cidade, nameof(cidade));
            Guard.AgainstNullOrWhiteSpace(estado, nameof(estado));
            Guard.AgainstNullOrWhiteSpace(pais, nameof(pais));

            Cep = cep;
            Logradouro = logradouro;
            Numero = numero;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            Pais = pais;
            Complemento = complemento;
        }
    }


}
