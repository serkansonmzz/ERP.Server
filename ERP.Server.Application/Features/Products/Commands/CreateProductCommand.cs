using ERP.Server.Domain.Entities;
using ERP.Server.Domain.Entities.Enums;
using ERP.Server.Domain.Interfaces;
using MediatR;

namespace ERP.Server.Application.Features.Products.Commands;

public sealed record CreateProductCommand(
    string Name,
    string ProductTypeValue) : IRequest<Guid>;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // SmartEnum'u gelen string değerden buluyoruz.
        if (!ProductType.TryFromValue(request.ProductTypeValue, out var productType))
        {
            // Burada özel bir exception fırlatmak daha doğru olacaktır.
            throw new ArgumentException("Geçersiz ürün tipi.", nameof(request.ProductTypeValue));
        }

        var product = new Product(request.Name, productType!);

        await _unitOfWork.ProductRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
