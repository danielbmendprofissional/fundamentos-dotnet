namespace Galo.Domain.Competicoes.Enums
{
    public enum TipoCampeonato { Brasileirao = 1, CopaDoBrasil = 2, Libertadores = 3, Sulamericana = 4, Mineiro = 5 }

    public enum StatusCampeonato { EmAndamento = 1, Encerrado = 2 }

    public enum StatusPartida { Agendada = 1, EmAndamento = 2, Encerrada = 3, Cancelada = 4 }

    /// <summary>Gol contra: o gol é creditado ao clube beneficiado, mas o autor é do adversário.</summary>
    public enum TipoGol { Normal = 1, Penalti = 2, FaltaDireta = 3, Contra = 4 }
}
