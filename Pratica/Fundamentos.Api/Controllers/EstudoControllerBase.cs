using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Base dos endpoints de estudo: enquanto o conceito não é implementado, responde 501.</summary>
[ApiController]
public abstract class EstudoControllerBase : ControllerBase
{
    protected IActionResult NaoImplementado() =>
        StatusCode(StatusCodes.Status501NotImplemented, new
        {
            status = "TODO",
            endpoint = $"{Request.Method} {Request.Path}"
        });
}
