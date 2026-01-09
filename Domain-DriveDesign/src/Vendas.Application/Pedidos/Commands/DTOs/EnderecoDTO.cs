using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Pedidos.Commands.DTOs
{
    public sealed record EnderecoDTO(
        string Cep,
        string Logradouro,
        string Bairro,
        string Cidade,
        string Estado,
        string Pais,
        string? Complemento
    );
}
