using ERP.Server.Application.Common.Results;
using ERP.Server.Application.Features.Products.Commands;
using ERP.Server.Application.Features.Products.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace ERP.Server.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : BaseApiController
{
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ILogger<ProductsController> logger)
    {
        _logger = logger;
    }
    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="request">Product details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created product details</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creating product with name: {ProductName}", request.Name);

        try
        {
            var response = await Mediator.Send(request, cancellationToken);
            
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetById), new { id = response.Data }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product with name: {ProductName}", request.Name);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the product");
        }
    }

    /// <summary>
    /// Gets all products
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all products</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting product with ID: {ProductId}", id);

        try
        {
            var query = new GetProductByIdQuery { Id = id };
            var response = await Mediator.Send(query, cancellationToken);
            
            if (!response.IsSuccess || response.Data == null)
            {
                return NotFound($"Product with ID {id} not found");
            }

            return Ok(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product with ID: {ProductId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the product");
        }
    }

    /// <summary>
    /// Gets all products
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all products</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllProductsQuery();
        _logger.LogInformation("Getting all products");

        try
        {
            var response = await Mediator.Send(query, cancellationToken);
            
            if (!response.IsSuccess || response.Data == null)
            {
                return NotFound("No products found");
            }

            return Ok(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all products");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving products");
        }
    }
}
