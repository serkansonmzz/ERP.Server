using ERP.Server.Application.Common.Results;
using ERP.Server.Domain.Entities.Enums;
using ERP.Server.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Server.Application.Features.Products.Commands;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    string ProductTypeValue) : IRequest<Result>;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.ProductCommandRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
                return Result.Failure("Product not found");

            if (!ProductType.TryFromValue(request.ProductTypeValue, out var type))
                return Result.Failure("Invalid product type");

            product.Update(request.Name, type!, request.Description);

            await _unitOfWork.ProductCommandRepository.UpdateAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Product updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {Id}", request.Id);
            return Result.Failure("Error updating product", new List<string> { ex.Message });
        }
    }
}
