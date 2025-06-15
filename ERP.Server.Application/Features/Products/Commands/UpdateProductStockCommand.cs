using ERP.Server.Application.Common.Results;
using ERP.Server.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Server.Application.Features.Products.Commands;

public sealed record UpdateProductStockCommand(Guid Id, int Quantity) : IRequest<Result>;

public sealed class UpdateProductStockCommandHandler : IRequestHandler<UpdateProductStockCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProductStockCommandHandler> _logger;

    public UpdateProductStockCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateProductStockCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateProductStockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.ProductCommandRepository.UpdateStockAsync(request.Id, request.Quantity, cancellationToken);
            return Result.Success("Stock updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock of product {Id}", request.Id);
            return Result.Failure("Error updating stock", new List<string> { ex.Message });
        }
    }
}
