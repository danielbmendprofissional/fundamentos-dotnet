namespace Galo.Estudos.Common
{
    /// <summary>Uma variação medida dentro de um experimento (ex.: "Com tracking" x "Sem tracking").</summary>
    public sealed record Medicao(
        string Nome,
        string Descricao,
        double DuracaoMs,
        double AlocadoKb,
        int ComandosSql,
        IReadOnlyDictionary<string, object?> Detalhes,
        IReadOnlyList<string>? AmostrasSql = null);

    /// <summary>Resultado completo de um experimento do laboratório.</summary>
    public sealed record ExperimentoResultado(
        string Titulo,
        string Conceito,
        IReadOnlyList<Medicao> Medicoes,
        IReadOnlyList<string> Observacoes);
}
