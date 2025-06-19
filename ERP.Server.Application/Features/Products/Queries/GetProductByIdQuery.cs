using ERP.Server.Application.Common.Results;
using ERP.Server.Application.Features.Products.Dtos;
using ERP.Server.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Server.Application.Features.Products.Queries;

public class GetProductByIdQuery : IRequest<Result<ProductDto>>
{
    public Guid Id { get; set; }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetProductByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.ProductQueries.GetByIdAsync(request.Id, cancellationToken);
            
            if (product == null)
            {
                return Result<ProductDto>.Failure(
                    $"Product with ID {request.Id} not found",
                    new List<string> { "Product not found" });
            }

            // Map the product to ProductDto
            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                // Map other properties as needed
            };

            return Result<ProductDto>.Success(
                productDto,
                "Product retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving product with ID: {ProductId}", request.Id);
            return Result<ProductDto>.Failure(
                "An error occurred while retrieving the product",
                new List<string> { ex.Message });
        }
    }
}
