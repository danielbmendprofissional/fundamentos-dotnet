using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Infrastructure.Clientes.Persistence.QueryModels
{
    public sealed class EnderecoPrincipalModel
    {
        public Guid Id { get; }
        public Guid ClienteId { get; init; }
        public string Cep { get;} = string.Empty;
        public string Logradouro { get; } = string.Empty;
        public string Numero { get; } = string.Empty;
        public string Bairro { get; } = string.Empty;
        public string Cidade { get; } = string.Empty;
        public string Estado { get; } = string.Empty;
        public string Pais { get; } = string.Empty;
        public string? Complemento { get; }
    }
}
