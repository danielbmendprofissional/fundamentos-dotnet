using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Clientes.Commands.DTOs;
using Vendas.Domain.Clientes.ValueObjects;

namespace Vendas.Application.Clientes.Commands.AdicionarEndereco
{
    public sealed record AdicionarEnderecoCommand(
            Guid ClienteId,
            EnderecoDTO Endereco
        ): IRequest;
}
