using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Clientes.Commands.AtualizarPerfil
{
    public sealed record AtualizarPerfilCommand(
            Guid Id,
            string Nome,
            string Email,
            string Telefone,
            long SexoId,
            long EstadoCivilId
        ) : IRequest;
}
