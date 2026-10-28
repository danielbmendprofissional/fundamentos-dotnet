using System.ComponentModel;

namespace Vendas.Domain.Clientes.Enums
{
    //É Indicador - tabela de indicador (está ENUM para fins didáticos)
    public enum EstadoCivil
    {
        [Description("Não informado")]
        NaoInformado = 0,
        [Description("Solteiro")]
        Solteiro = 1,
        [Description("Casado")]
        Casado = 2,
        [Description("Divorciado")]
        Divorciado = 3,
        [Description("Viúvo")]
        Viuvo = 4,
        [Description("União Estável")]
        UniaoEstavel = 5
    }
}
