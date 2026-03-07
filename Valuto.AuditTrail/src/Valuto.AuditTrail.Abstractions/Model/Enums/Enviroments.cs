using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Model.Enums
{
    public enum Enviroments
    {
        /// <summary>
        ///Ambiente para execução do rabbit utilizando queue em produção
        /// </summary>
        [Description("Producao")]
        Producao = 1,

        /// <summary>
        ///Ambiente para execução do rabbit utilizando queue em homologação
        /// </summary>
        [Description("Homologacao")]
        Homologacao = 2,
    }
}
