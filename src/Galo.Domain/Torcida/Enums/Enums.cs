namespace Galo.Domain.Torcida.Enums
{
    /// <summary>
    /// Plano de sócio-torcedor. (Original do projeto: "indicador" seria melhor que enum em um
    /// sistema real - aqui é educacional, ver Conteudo/1 - Fundamentos/8 - IndicadorXEnum.txt.)
    /// </summary>
    public enum PlanoSocio { Nenhum = 0, Bronze = 1, Prata = 2, Ouro = 3, Diamante = 4 }

    public enum StatusTorcedor { Ativo = 1, Bloqueado = 2 }
}
