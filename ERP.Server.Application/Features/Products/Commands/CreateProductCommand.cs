// ERP.Server.Application/Features/Products/Commands/CreateProductCommand.cs
using ERP.Server.Domain.Entities;
using ERP.Server.Domain.Entities.Enums;
using MediatR;

namespace ERP.Server.Application.Features.Products.Commands;

public sealed record CreateProductCommand(
    string Name,
    string ProductTypeValue) : IRequest<Guid>;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
         
          _logger.LogInformation("Creating product with name: {ProductName}", request.Name);
       
          // Hata varsa pipeline buraya hiç gelmeyecek.
           ProductType.TryFromValue(request.ProductTypeValue, out var productType)
       

            var product = new Product(request.Name, productType!);
            await _unitOfWork.ProductRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        
         _logger.LogInformation("Product created with ID: {ProductId}", productId);

        return product.Id;
    }
}