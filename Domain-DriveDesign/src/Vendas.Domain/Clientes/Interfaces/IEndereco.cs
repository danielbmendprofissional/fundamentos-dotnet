using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Domain.Clientes.Interfaces
{
    public interface IEndereco
    {
        public Guid Id { get; }
        public string Cep { get; }
        public string Logradouro { get; }
        public string Numero { get; }
        public string Bairro { get; }
        public string Cidade { get; }
        public string Estado { get; }
        public string Pais { get; }
        public string Complemento { get; }
    }
}
