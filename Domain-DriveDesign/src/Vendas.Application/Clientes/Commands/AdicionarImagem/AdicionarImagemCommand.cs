using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Clientes.Commands.AdicionarImagem
{
    public sealed record AdicionarImagemCommand(
            Guid Id,
            string Url,
            int Ordem
        ) : IRequest;
}
