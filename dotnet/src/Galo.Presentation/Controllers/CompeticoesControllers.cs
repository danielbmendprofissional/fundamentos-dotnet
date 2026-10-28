using Galo.Application.Competicoes.Commands;
using Galo.Application.Competicoes.Queries;
using Galo.Domain.Competicoes.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Galo.Presentation.Controllers
{
    public sealed record CadastrarClubeRequest(string Nome, string Sigla, string Cidade, string Uf, int AnoFundacao, bool EhOGalo = false);
    public sealed record CadastrarEstadioRequest(string Nome, string Cidade, int Capacidade);
    public sealed record CriarCampeonatoRequest(string Nome, int Temporada, TipoCampeonato Tipo);

    public sealed record AgendarPartidaRequest(
        Guid CampeonatoId, Guid MandanteId, Guid VisitanteId, Guid EstadioId, DateTime DataHoraUtc, int Rodada);
    public sealed record AdicionarSetorRequest(string Nome, int Capacidade, decimal Preco);
    public sealed record RegistrarGolRequest(Guid ClubeBeneficiadoId, Guid? JogadorId, int Minuto, TipoGol Tipo = TipoGol.Normal);
    public sealed record CancelarPartidaRequest(string Motivo);

    /// <summary>Cadastros de apoio: clubes, estádios e campeonatos (+ classificação).</summary>
    [ApiController]
    [Route("api/v1")]
    public sealed class CatalogoController : ControllerBase
    {
        private readonly ISender _sender;
        public CatalogoController(ISender sender) => _sender = sender;

        [HttpGet("clubes")]
        public async Task<IActionResult> ListarClubes(CancellationToken ct) => Ok(await _sender.Send(new ListarClubesQuery(), ct));

        [HttpPost("clubes")]
        public async Task<IActionResult> CadastrarClube(CadastrarClubeRequest r, CancellationToken ct)
        {
            var id = await _sender.Send(new CadastrarClubeCommand(r.Nome, r.Sigla, r.Cidade, r.Uf, r.AnoFundacao, r.EhOGalo), ct);
            return Created("api/v1/clubes", new { id });
        }

        [HttpGet("estadios")]
        public async Task<IActionResult> ListarEstadios(CancellationToken ct) => Ok(await _sender.Send(new ListarEstadiosQuery(), ct));

        [HttpPost("estadios")]
        public async Task<IActionResult> CadastrarEstadio(CadastrarEstadioRequest r, CancellationToken ct)
        {
            var id = await _sender.Send(new CadastrarEstadioCommand(r.Nome, r.Cidade, r.Capacidade), ct);
            return Created("api/v1/estadios", new { id });
        }

        [HttpGet("campeonatos")]
        public async Task<IActionResult> ListarCampeonatos(CancellationToken ct) => Ok(await _sender.Send(new ListarCampeonatosQuery(), ct));

        [HttpPost("campeonatos")]
        public async Task<IActionResult> CriarCampeonato(CriarCampeonatoRequest r, CancellationToken ct)
        {
            var id = await _sender.Send(new CriarCampeonatoCommand(r.Nome, r.Temporada, r.Tipo), ct);
            return Created("api/v1/campeonatos", new { id });
        }

        /// <summary>Tabela de classificação (cacheada em 2 níveis: memória + Redis; invalidada quando uma partida encerra).</summary>
        [HttpGet("campeonatos/{id:guid}/classificacao")]
        public async Task<IActionResult> Classificacao(Guid id, CancellationToken ct)
            => Ok(await _sender.Send(new ObterClassificacaoQuery(id), ct));
    }

    [ApiController]
    [Route("api/v1/partidas")]
    public sealed class PartidasController : ControllerBase
    {
        private readonly ISender _sender;
        public PartidasController(ISender sender) => _sender = sender;

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] Guid? campeonatoId, [FromQuery] Guid? clubeId, [FromQuery] StatusPartida? status,
            [FromQuery] int pagina = 1, [FromQuery] int tamanho = 20, CancellationToken ct = default)
            => Ok(await _sender.Send(new ListarPartidasQuery(campeonatoId, clubeId, status, pagina, tamanho), ct));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
            => Ok(await _sender.Send(new ObterPartidaQuery(id), ct));

        [HttpPost]
        public async Task<IActionResult> Agendar(AgendarPartidaRequest r, CancellationToken ct)
        {
            var id = await _sender.Send(new AgendarPartidaCommand(r.CampeonatoId, r.MandanteId, r.VisitanteId, r.EstadioId, r.DataHoraUtc, r.Rodada), ct);
            return CreatedAtAction(nameof(Obter), new { id }, new { id });
        }

        [HttpPost("{id:guid}/setores")]
        public async Task<IActionResult> AdicionarSetor(Guid id, AdicionarSetorRequest r, CancellationToken ct)
        {
            var setorId = await _sender.Send(new AdicionarSetorCommand(id, r.Nome, r.Capacidade, r.Preco), ct);
            return Created($"api/v1/partidas/{id}", new { id = setorId });
        }

        [HttpPost("{id:guid}/iniciar")]
        public async Task<IActionResult> Iniciar(Guid id, CancellationToken ct)
        {
            await _sender.Send(new IniciarPartidaCommand(id), ct);
            return NoContent();
        }

        [HttpPost("{id:guid}/gols")]
        public async Task<IActionResult> RegistrarGol(Guid id, RegistrarGolRequest r, CancellationToken ct)
        {
            await _sender.Send(new RegistrarGolCommand(id, r.ClubeBeneficiadoId, r.JogadorId, r.Minuto, r.Tipo), ct);
            return NoContent();
        }

        [HttpPost("{id:guid}/encerrar")]
        public async Task<IActionResult> Encerrar(Guid id, CancellationToken ct)
        {
            await _sender.Send(new EncerrarPartidaCommand(id), ct);
            return NoContent();
        }

        [HttpPost("{id:guid}/cancelar")]
        public async Task<IActionResult> Cancelar(Guid id, CancelarPartidaRequest r, CancellationToken ct)
        {
            await _sender.Send(new CancelarPartidaCommand(id, r.Motivo), ct);
            return NoContent();
        }
    }
}
