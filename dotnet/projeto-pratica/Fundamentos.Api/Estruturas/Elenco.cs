public class Elenco
{
    private readonly List<string> _jogadores = new();

    // vitrine ERRADA: devolve a própria lista, só "disfarçada" de somente leitura
    public IReadOnlyCollection<string> JogadoresVulneravel => _jogadores;

    // vitrine CERTA: devolve um invólucro que não deixa alterar
    public IReadOnlyCollection<string> JogadoresSeguro => _jogadores.AsReadOnly();

    public void Contratar(string nome) => _jogadores.Add(nome);
}
