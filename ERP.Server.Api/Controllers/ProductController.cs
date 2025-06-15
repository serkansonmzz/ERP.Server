using ERP.Server.Application.Features.Products.Commands;
using ERP.Server.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Server.Api.Controllers;

public class ProductController : BaseApiController
{
    private readonly ILogger<ProductController> _logger;

    public ProductController(ISender mediator, ILogger<ProductController> logger)
        : base(mediator)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetAllProductsQuery());
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetProductByIdQuery(id));
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest("Id mismatch");

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{id}/price")]
    public async Task<IActionResult> UpdatePrice(Guid id, UpdateProductPriceCommand command)
    {
        if (id != command.Id)
            return BadRequest("Id mismatch");

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateStock(Guid id, UpdateProductStockCommand command)
    {
        if (id != command.Id)
            return BadRequest("Id mismatch");

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteProductCommand(id));
        return HandleResult(result);
    }
}
