using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Estado mantido em memória no processo, sem banco.</summary>
[Route("api/memoria/in-memory")]
[Tags("02.01 - Dados em memória")]
public class InMemoryController(ISocioRepository repositorio) : EstudoControllerBase
{
    [HttpGet]
    public IActionResult Listar()
    {
        var socios = repositorio.Listar();
        return Ok(new { total = socios.Count, socios });
    }

    [HttpPost]
    public IActionResult Criar([FromBody] CriarSocioRequest request) =>
        StatusCode(StatusCodes.Status201Created, repositorio.Adicionar(request.Nome));

    [HttpDelete("{id}")]
    public IActionResult Remover(int id) =>
        repositorio.Remover(id) ? NoContent() : NotFound(new { mensagem = $"sócio {id} não existe" });
}
