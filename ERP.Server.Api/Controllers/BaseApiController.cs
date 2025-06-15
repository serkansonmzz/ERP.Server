using ERP.Server.Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Server.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected ISender Mediator { get; }

    protected BaseApiController(ISender mediator)
    {
        Mediator = mediator;
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return Ok(result.Message);
        return BadRequest(new { result.Message, result.Errors });
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Data);
        return BadRequest(new { result.Message, result.Errors });
    }
}
