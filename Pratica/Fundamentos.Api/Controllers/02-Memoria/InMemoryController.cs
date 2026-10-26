using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Estado mantido em memória no processo, sem banco.</summary>
[Route("api/memoria/in-memory")]
[Tags("02.01 - Dados em memória")]
public class InMemoryController : EstudoControllerBase
{
    /// <summary>CRUD sobre uma List estática: listar.</summary>
    [HttpGet]
    public IActionResult Listar() => NaoImplementado();

    /// <summary>CRUD em memória: criar.</summary>
    [HttpPost]
    public IActionResult Criar() => NaoImplementado();

    /// <summary>CRUD em memória: remover por id.</summary>
    [HttpDelete("{id}")]
    public IActionResult Remover() => NaoImplementado();

    /// <summary>O que acontece com o estado quando a aplicação reinicia? E com vários usuários?</summary>
    [HttpGet("ciclo-de-vida")]
    public IActionResult CicloDeVida() => NaoImplementado();

    /// <summary>Tempo de vida de serviços na injeção de dependência.</summary>
    [HttpGet("singleton-scoped-transient")]
    public IActionResult SingletonScopedTransient() => NaoImplementado();
}
