using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Clientes.Commands.DTOs;

namespace Vendas.Application.Clientes.Commands.AlterarEndereco
{
    public sealed record class AlterarEnderecoCommand(
        Guid ClienteId,
        Guid EndereocId,
        EnderecoDTO Endereco
        ): IRequest;
}
