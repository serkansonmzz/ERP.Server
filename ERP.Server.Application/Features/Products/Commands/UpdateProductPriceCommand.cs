using ERP.Server.Application.Common.Results;
using ERP.Server.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Server.Application.Features.Products.Commands;

public sealed record UpdateProductPriceCommand(Guid Id, decimal NewPrice) : IRequest<Result>;

public sealed class UpdateProductPriceCommandHandler : IRequestHandler<UpdateProductPriceCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProductPriceCommandHandler> _logger;

    public UpdateProductPriceCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateProductPriceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateProductPriceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.ProductCommandRepository.UpdatePriceAsync(request.Id, request.NewPrice, cancellationToken);
            return Result.Success("Price updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating price of product {Id}", request.Id);
            return Result.Failure("Error updating price", new List<string> { ex.Message });
        }
    }
}
