// ERP.Server.Application/Features/Products/Commands/CreateProductCommand.cs
using ERP.Server.Application.Common.Results;
using ERP.Server.Domain.Entities;
using ERP.Server.Domain.Entities.Enums;
using ERP.Server.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Server.Application.Features.Products.Commands;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    string ProductTypeValue) : IRequest<Result<Guid>>;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IUnitOfWork unitOfWork, 
        ILogger<CreateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CreateProductCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating product with name: {ProductName}", request.Name);

            // Validasyon
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result<Guid>.Failure(
                    "Ürün adı boş olamaz",
                    new List<string> { "Name alanı zorunludur" });
            }

            if (!ProductType.TryFromValue(request.ProductTypeValue, out var productType))
            {
                return Result<Guid>.Failure(
                    "Geçersiz ürün tipi",
                    new List<string> { "Lütfen geçerli bir ürün tipi seçiniz" });
            }

            var product = new Product(request.Name, productType!)
            {
                Description = request.Description
            };

            await _unitOfWork.ProductRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        
            _logger.LogInformation("Product created with ID: {ProductId}", product.Id);

            return Result<Guid>.Success(
                product.Id,
                "Ürün başarıyla oluşturuldu");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ürün oluşturulurken bir hata oluştu");
            return Result<Guid>.Failure(
                "Ürün oluşturulurken bir hata oluştu",
                new List<string> { ex.Message });
        }
    }
}