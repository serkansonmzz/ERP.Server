using ERP.Server.Application.Common.Results;
using ERP.Server.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Server.Application.Features.Products.Commands;

public sealed record DeleteProductCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.ProductCommandRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
                return Result.Failure("Product not found");

            await _unitOfWork.ProductCommandRepository.DeleteAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Product deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {Id}", request.Id);
            return Result.Failure("Error deleting product", new List<string> { ex.Message });
        }
    }
}
