using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces
{
    public interface IEmailService
    {
        Task EnviarEmail(string recipient, string subject, string message, CancellationToken cancellationToken);
    }
}
