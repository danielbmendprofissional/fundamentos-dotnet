using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Clientes.Commands.DTOs;

namespace Vendas.Application.Clientes.Commands.Criar
{
    public sealed record CriarCommand(
            string Nome,
            string Cpf,
            string Email,
            string Telefone,
            long SexoId,
            long EstadoCivilId,
            EnderecoDTO EnderecoPrincipal
        ) : IRequest;
}
