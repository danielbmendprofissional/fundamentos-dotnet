using System.ComponentModel;

namespace Vendas.Domain.Clientes.Enums
{
    //É Indicador - tabela de indicador (está ENUM para fins didáticos)
    public enum Sexo
    {
        [Description("Não Informado")]
        NaoInformado = 0,
        [Description("Masculino")]
        Masculino = 1,
        [Description("Feminino")]
        Feminino = 2,
        [Description("Outro")]
        Outro = 3,
    }
}
