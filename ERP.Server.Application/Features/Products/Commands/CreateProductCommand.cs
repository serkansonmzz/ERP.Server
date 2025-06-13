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

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
       // FluentValidation bu kontrolü bizim için zaten yaptı.
    // Hata varsa pipeline buraya hiç gelmeyecek.
           ProductType.TryFromValue(request.ProductTypeValue, out var productType)
       

            var product = new Product(request.Name, productType!);
            await _unitOfWork.ProductRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}