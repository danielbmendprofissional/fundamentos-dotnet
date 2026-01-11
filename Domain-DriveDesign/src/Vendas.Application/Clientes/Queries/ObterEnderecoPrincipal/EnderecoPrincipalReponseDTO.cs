using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Clientes.Queries.ObterEnderecoPrincipal
{
    public sealed record EnderecoPrincipalReponseDTO(
        Guid ClienteId,
        Guid EnderecoId,
        string Cep,
        string Logradouro,
        string Bairro,
        string Cidade,
        string Estado,
        string Pais,
        string? Complemento
        );
}
