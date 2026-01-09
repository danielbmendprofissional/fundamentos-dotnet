using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Validations;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Domain.Clientes.ValueObjects
{
    public sealed class DadosEndereco : ValueObject
    {
        public string Cep { get; }
        public string Logradouro { get; }
        public string Numero { get; }
        public string Bairro { get; }
        public string Cidade { get; }
        public string Estado { get; }
        public string Pais { get; }
        public string Complemento { get; }

        private DadosEndereco(
            string cep,
            string logradouro,
            string numero,
            string bairro,
            string cidade,
            string estado,
            string pais,
            string complemento)
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
        public static DadosEndereco Criar(string cep, string logradouro, string numero,
                        string bairro, string estado, string cidade, string pais, string complemento = "")
        {
            return new DadosEndereco(cep, logradouro, complemento, bairro, estado, cidade, pais, complemento);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Cep;
            yield return Logradouro;
            yield return Complemento;
            yield return Bairro;
            yield return Estado;
            yield return Cidade;
            yield return Pais;
        }
    }


}
