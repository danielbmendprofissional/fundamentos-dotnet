using System.Collections.Concurrent;

public record Socio(int Id, string Nome);
public record CriarSocioRequest(string Nome);

public interface ISocioRepository
{
    Socio Adicionar(string nome);
    IReadOnlyCollection<Socio> Listar();
    bool Remover(int id);
}

public class SocioRepositoryEmMemoria : ISocioRepository
{
    private readonly ConcurrentDictionary<int, Socio> _socios = new();
    private int _ultimoId;

    public Socio Adicionar(string nome)
    {
        int id = Interlocked.Increment(ref _ultimoId);    // id único mesmo com requisições simultâneas
        var socio = new Socio(id, nome);
        _socios[id] = socio;
        return socio;
    }

    public IReadOnlyCollection<Socio> Listar() => _socios.Values.OrderBy(s => s.Id).ToList();

    public bool Remover(int id) => _socios.TryRemove(id, out _);
}
