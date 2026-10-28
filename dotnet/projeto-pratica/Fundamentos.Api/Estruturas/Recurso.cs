public class Recurso : IDisposable
{
    private readonly List<string> _log;
    public string Nome { get; }

    public Recurso(string nome, List<string> log)
    {
        Nome = nome;
        _log = log;
        _log.Add($"{nome}: aberto");
    }

    public void Usar() => _log.Add($"{Nome}: usando");

    public void Dispose() => _log.Add($"{Nome}: liberado (Dispose)");
}
