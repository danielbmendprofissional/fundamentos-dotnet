using Galo.Application.Elenco.Commands;
using Galo.Application.Elenco.Queries;
using Galo.Domain.Elenco.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Galo.Presentation.Controllers
{
    public sealed record CadastrarJogadorRequest(
        string Nome, string? Apelido, int Camisa, Posicao Posicao,
        DateOnly DataNascimento, string Nacionalidade, decimal ValorMercado);

    public sealed record AtualizarValorMercadoRequest(decimal NovoValor);
    public sealed record RegistrarContratoRequest(DateOnly Inicio, DateOnly Fim, decimal SalarioMensal, decimal? ClausulaRescisoria);
    public sealed record LesionarJogadorRequest(string Descricao);

    /// <summary>
    /// Controller FINO: só traduz HTTP <-> MediatR. Nenhuma regra de negócio aqui.
    /// </summary>
    [ApiController]
    [Route("api/v1/jogadores")]
    public sealed class JogadoresController : ControllerBase
    {
        private readonly ISender _sender;
        public JogadoresController(ISender sender) => _sender = sender;

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] Posicao? posicao, [FromQuery] StatusJogador? status,
            [FromQuery] int pagina = 1, [FromQuery] int tamanho = 20, CancellationToken ct = default)
            => Ok(await _sender.Send(new ListarJogadoresQuery(posicao, status, pagina, tamanho), ct));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
            => Ok(await _sender.Send(new ObterJogadorQuery(id), ct));

        [HttpPost]
        public async Task<IActionResult> Cadastrar(CadastrarJogadorRequest r, CancellationToken ct)
        {
            var id = await _sender.Send(new CadastrarJogadorCommand(
                r.Nome, r.Apelido, r.Camisa, r.Posicao, r.DataNascimento, r.Nacionalidade, r.ValorMercado), ct);
            return CreatedAtAction(nameof(Obter), new { id }, new { id });
        }

        [HttpPut("{id:guid}/valor-mercado")]
        public async Task<IActionResult> AtualizarValorMercado(Guid id, AtualizarValorMercadoRequest r, CancellationToken ct)
        {
            await _sender.Send(new AtualizarValorMercadoCommand(id, r.NovoValor), ct);
            return NoContent();
        }

        [HttpPost("{id:guid}/contratos")]
        public async Task<IActionResult> RegistrarContrato(Guid id, RegistrarContratoRequest r, CancellationToken ct)
        {
            var contratoId = await _sender.Send(new RegistrarContratoCommand(id, r.Inicio, r.Fim, r.SalarioMensal, r.ClausulaRescisoria), ct);
            return Created($"api/v1/jogadores/{id}", new { id = contratoId });
        }

        [HttpPost("{id:guid}/lesao")]
        public async Task<IActionResult> Lesionar(Guid id, LesionarJogadorRequest r, CancellationToken ct)
        {
            await _sender.Send(new LesionarJogadorCommand(id, r.Descricao), ct);
            return NoContent();
        }

        [HttpPost("{id:guid}/recuperacao")]
        public async Task<IActionResult> Recuperar(Guid id, CancellationToken ct)
        {
            await _sender.Send(new RecuperarJogadorCommand(id), ct);
            return NoContent();
        }
    }
}
