using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Persistence.Service
{
    internal class ExempleInfraService
    {
        //Aqui ficaria os external services (consultas a serviços externos)

        /*   
            SOBRE ONDE FICA A INTERFACE DE EXTERNAL SERVICE:

            - Application: quando o service é algo "extra" a regra de negócio, como enviar um e-mail após concluir cadastro.

            - Domain: quando a regra de negócio depende do service para existir.
        */

    }
}
