namespace Galo.Infrastructure.Persistence.QueryModels
{
    // ========================================================================================
    // QUERY MODELS (lado de LEITURA do CQRS)
    //
    // Classes ANÊMICAS (só dados) mapeadas nas MESMAS tabelas do modelo de escrita, mas com tipos primitivos
    // (sem Value Objects) e com propriedades de navegação para facilitar JOINs nas projeções.
    //
    // Por que separar?  - O modelo de escrita protege invariantes (setters privados, VOs); leitura quer só dados rápidos.
    //                   - Consultas usam AsNoTracking + Select direto para DTO: sem hidratar agregados.
    //
    // As navegações são "virtual" e as classes não são sealed DE PROPÓSITO: o laboratório (Galo.Estudos) reutiliza
    // este mesmo modelo com UseLazyLoadingProxies() para demonstrar lazy loading. No GaloQueryDbContext normal
    // (sem proxies) "virtual" não tem efeito nenhum.
    // ========================================================================================

    public class JogadorQM
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Apelido { get; set; }
        public int Camisa { get; set; }
        public string Posicao { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateOnly DataNascimento { get; set; }
        public string Nacionalidade { get; set; } = string.Empty;
        public decimal ValorMercado { get; set; }
        public DateTime DataCriacao { get; set; }

        public virtual ICollection<ContratoQM> Contratos { get; set; } = new List<ContratoQM>();
    }

    public class ContratoQM
    {
        public Guid Id { get; set; }
        public Guid JogadorId { get; set; }
        public DateOnly Inicio { get; set; }
        public DateOnly Fim { get; set; }
        public decimal SalarioMensal { get; set; }
        public decimal? ClausulaRescisoria { get; set; }

        public virtual JogadorQM? Jogador { get; set; }
    }

    public class ClubeQM
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Sigla { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
        public int AnoFundacao { get; set; }
        public bool EhOGalo { get; set; }
    }

    public class EstadioQM
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public int Capacidade { get; set; }
    }

    public class CampeonatoQM
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Temporada { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class PartidaQM
    {
        public Guid Id { get; set; }
        public Guid CampeonatoId { get; set; }
        public Guid MandanteId { get; set; }
        public Guid VisitanteId { get; set; }
        public Guid EstadioId { get; set; }
        public DateTime DataHoraUtc { get; set; }
        public int Rodada { get; set; }
        public string Status { get; set; } = string.Empty;
        public int GolsMandante { get; set; }
        public int GolsVisitante { get; set; }

        public virtual CampeonatoQM Campeonato { get; set; } = null!;
        public virtual ClubeQM Mandante { get; set; } = null!;
        public virtual ClubeQM Visitante { get; set; } = null!;
        public virtual EstadioQM Estadio { get; set; } = null!;
        public virtual ICollection<GolQM> Gols { get; set; } = new List<GolQM>();
        public virtual ICollection<SetorQM> Setores { get; set; } = new List<SetorQM>();
    }

    public class GolQM
    {
        public Guid Id { get; set; }
        public Guid PartidaId { get; set; }
        public Guid ClubeId { get; set; }
        public Guid? JogadorId { get; set; }
        public int Minuto { get; set; }
        public string Tipo { get; set; } = string.Empty;

        public virtual PartidaQM Partida { get; set; } = null!;
        public virtual ClubeQM Clube { get; set; } = null!;
        public virtual JogadorQM? Jogador { get; set; }
    }

    public class SetorQM
    {
        public Guid Id { get; set; }
        public Guid PartidaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Capacidade { get; set; }
        public int Disponiveis { get; set; }
        public decimal Preco { get; set; }
    }

    public class TorcedorQM
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public DateOnly DataNascimento { get; set; }
        public string Plano { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid? EnderecoPrincipalId { get; set; }

        public virtual ICollection<EnderecoQM> Enderecos { get; set; } = new List<EnderecoQM>();
        public virtual ICollection<CompraQM> Compras { get; set; } = new List<CompraQM>();
    }

    public class EnderecoQM
    {
        public Guid Id { get; set; }
        public Guid TorcedorId { get; set; }
        public string Cep { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string? Complemento { get; set; }
    }

    public class CompraQM
    {
        public Guid Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public Guid TorcedorId { get; set; }
        public Guid PartidaId { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool ReservaConfirmada { get; set; }
        public string? MotivoCancelamento { get; set; }
        public DateTime DataCriacao { get; set; }

        public virtual TorcedorQM Torcedor { get; set; } = null!;
        public virtual PartidaQM Partida { get; set; } = null!;
        public virtual ICollection<ItemCompraQM> Itens { get; set; } = new List<ItemCompraQM>();
        public virtual ICollection<PagamentoCompraQM> Pagamentos { get; set; } = new List<PagamentoCompraQM>();
    }

    public class ItemCompraQM
    {
        public Guid Id { get; set; }
        public Guid CompraId { get; set; }
        public string Setor { get; set; } = string.Empty;
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
        public decimal PorcentagemDesconto { get; set; }
        public decimal ValorTotal { get; set; }
    }

    public class PagamentoCompraQM
    {
        public Guid Id { get; set; }
        public Guid CompraId { get; set; }
        public string Metodo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime? DataPagamento { get; set; }
        public string? CodigoTransacao { get; set; }
    }
}
