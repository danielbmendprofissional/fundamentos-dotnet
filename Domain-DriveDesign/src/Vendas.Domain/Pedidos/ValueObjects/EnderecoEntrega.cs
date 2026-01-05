using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Pedidos.ValueObjects
{
    public sealed class EnderecoEntrega : ValueObject
    {
        public Cep Cep { get; }
        public string Logradouro { get; private set; }
        public string Complemento { get; private set; }
        public string Bairro { get; private set; }
        public string Estado { get; private set; }
        public string Cidade { get; private set; }
        public string Pais { get; private set; }

        private EnderecoEntrega(Cep cep, string logradouro, string complemento,
                                string bairro, string estado, string cidade, string pais)
        {
            Guard.AgainstNull(cep, nameof(cep));
            Guard.AgainstNullOrWhiteSpace(logradouro, nameof(logradouro));
            Guard.AgainstNullOrWhiteSpace(bairro, nameof(bairro));
            Guard.AgainstNullOrWhiteSpace(estado, nameof(estado));
            Guard.AgainstNullOrWhiteSpace(cidade, nameof(cidade));
            Guard.AgainstNullOrWhiteSpace(pais, nameof(pais));

            Cep = cep;
            Logradouro = logradouro;
            Complemento = complemento ?? string.Empty;
            Bairro = bairro;
            Estado = estado;
            Cidade = cidade;
            Pais = pais;
        }

        public static EnderecoEntrega Criar(string cep, string logradouro, string complemento,
                                string bairro, string estado, string cidade, string pais)
        {
            return new EnderecoEntrega(Cep.Criar(cep), logradouro, complemento, bairro, estado, cidade, pais);
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
