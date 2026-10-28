using Galo.Application.Ingressos.Commands;
using Galo.Application.Ingressos.Queries;
using Galo.Application.Torcida.Commands;
using Galo.Application.Torcida.Queries;
using Galo.Domain.Ingressos.Enums;
using Galo.Domain.Torcida.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Galo.Presentation.Controllers
{
    public sealed record CadastrarTorcedorRequest(string Nome, string Cpf, string Email, string? Telefone, DateOnly DataNascimento);
    public sealed record AlterarPlanoRequest(PlanoSocio Plano);
    public sealed record AdicionarEnderecoRequest(
        string Cep, string Logradouro, string Numero, string Bairro, string Cidade, string Estado,
        string Pais = "Brasil", string? Complemento = null, bool Principal = false);
    public sealed record BloquearTorcedorRequest(string Motivo);

    public sealed record CriarCompraRequest(Guid TorcedorId, Guid PartidaId, List<ItemCompraRequest> Itens);
    public sealed record IniciarPagamentoRequest(MetodoPagamento Metodo);
    public sealed record CancelarCompraRequest(string MotivoCodigo = "TorcedorDesistiu");

    [ApiController]
    [Route("api/v1/torcedores")]
    public sealed class TorcedoresController : ControllerBase
    {
        private readonly ISender _sender;
        public TorcedoresController(ISender sender) => _sender = sender;

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] PlanoSocio? plano, [FromQuery] int pagina = 1, [FromQuery] int tamanho = 20, CancellationToken ct = default)
            => Ok(await _sender.Send(new ListarTorcedoresQuery(plano, pagina, tamanho), ct));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
            => Ok(await _sender.Send(new ObterTorcedorQuery(id), ct));

        [HttpPost]
        public async Task<IActionResult> Cadastrar(CadastrarTorcedorRequest r, CancellationToken ct)
        {
            var id = await _sender.Send(new CadastrarTorcedorCommand(r.Nome, r.Cpf, r.Email, r.Telefone, r.DataNascimento), ct);
            return CreatedAtAction(nameof(Obter), new { id }, new { id });
        }

        [HttpPut("{id:guid}/plano")]
        public async Task<IActionResult> AlterarPlano(Guid id, AlterarPlanoRequest r, CancellationToken ct)
        {
            await _sender.Send(new AlterarPlanoSocioCommand(id, r.Plano), ct);
            return NoContent();
        }

        [HttpPost("{id:guid}/enderecos")]
        public async Task<IActionResult> AdicionarEndereco(Guid id, AdicionarEnderecoRequest r, CancellationToken ct)
        {
            var enderecoId = await _sender.Send(new AdicionarEnderecoCommand(id, r.Cep, r.Logradouro, r.Numero, r.Bairro,
                r.Cidade, r.Estado, r.Pais, r.Complemento, r.Principal), ct);
            return Created($"api/v1/torcedores/{id}", new { id = enderecoId });
        }

        [HttpPut("{id:guid}/enderecos/{enderecoId:guid}/principal")]
        public async Task<IActionResult> DefinirEnderecoPrincipal(Guid id, Guid enderecoId, CancellationToken ct)
        {
            await _sender.Send(new DefinirEnderecoPrincipalCommand(id, enderecoId), ct);
            return NoContent();
        }

        [HttpPost("{id:guid}/bloqueio")]
        public async Task<IActionResult> Bloquear(Guid id, BloquearTorcedorRequest r, CancellationToken ct)
        {
            await _sender.Send(new BloquearTorcedorCommand(id, r.Motivo), ct);
            return NoContent();
        }

        [HttpGet("{id:guid}/compras")]
        public async Task<IActionResult> Compras(Guid id, [FromQuery] int pagina = 1, [FromQuery] int tamanho = 20, CancellationToken ct = default)
            => Ok(await _sender.Send(new ListarComprasDoTorcedorQuery(id, pagina, tamanho), ct));
    }

    /// <summary>
    /// Fluxo: criar -> pagar (inicia a reserva ASSÍNCRONA no RabbitMQ) -> aguardar reserva -> confirmar pagamento -> emitir.
    /// </summary>
    [ApiController]
    [Route("api/v1/compras")]
    public sealed class ComprasController : ControllerBase
    {
        private readonly ISender _sender;
        public ComprasController(ISender sender) => _sender = sender;

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
            => Ok(await _sender.Send(new ObterCompraQuery(id), ct));

        [HttpPost]
        public async Task<IActionResult> Criar(CriarCompraRequest r, CancellationToken ct)
        {
            var id = await _sender.Send(new CriarCompraCommand(r.TorcedorId, r.PartidaId, r.Itens), ct);
            return CreatedAtAction(nameof(Obter), new { id }, new { id });
        }

        [HttpPost("{id:guid}/pagamentos")]
        public async Task<IActionResult> IniciarPagamento(Guid id, IniciarPagamentoRequest r, CancellationToken ct)
        {
            var pagamentoId = await _sender.Send(new IniciarPagamentoCommand(id, r.Metodo), ct);
            // 202 Accepted: a reserva dos ingressos continua em segundo plano (consulte GET /compras/{id}: reservaConfirmada).
            return Accepted(new { pagamentoId, observacao = "Reserva de ingressos em processamento assíncrono." });
        }

        [HttpPost("{id:guid}/pagamentos/{pagamentoId:guid}/confirmar")]
        public async Task<IActionResult> ConfirmarPagamento(Guid id, Guid pagamentoId, CancellationToken ct)
        {
            await _sender.Send(new ConfirmarPagamentoCommand(id, pagamentoId), ct);
            return NoContent();
        }

        [HttpPost("{id:guid}/pagamentos/{pagamentoId:guid}/recusar")]
        public async Task<IActionResult> RecusarPagamento(Guid id, Guid pagamentoId, CancellationToken ct)
        {
            await _sender.Send(new RecusarPagamentoCommand(id, pagamentoId), ct);
            return NoContent();
        }

        [HttpPost("{id:guid}/emitir")]
        public async Task<IActionResult> Emitir(Guid id, CancellationToken ct)
            => Ok(new { ingressos = await _sender.Send(new EmitirIngressosCommand(id), ct) });

        [HttpPost("{id:guid}/cancelar")]
        public async Task<IActionResult> Cancelar(Guid id, CancelarCompraRequest r, CancellationToken ct)
        {
            await _sender.Send(new CancelarCompraCommand(id, r.MotivoCodigo), ct);
            return NoContent();
        }
    }
}
