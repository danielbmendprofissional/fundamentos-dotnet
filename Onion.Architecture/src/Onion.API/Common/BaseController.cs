using Microsoft.AspNetCore.Mvc;

namespace Onion.API.Common
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        //protected IActionResult ProcessResult<T>(Result<T> result)
        //{
        //    if (result.IsSuccess)
        //        return Ok(result.Value);

        //    return BadRequest(new { errors = result.Errors });
        //}
    }
}
