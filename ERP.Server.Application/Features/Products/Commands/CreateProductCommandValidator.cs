using ERP.Server.Domain.Entities.Enums;
using FluentValidation;

namespace ERP.Server.Application.Features.Products.Commands;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Ürün adı boş olamaz.")
            .MinimumLength(3).WithMessage("Ürün adı en az 3 karakter olmalıdır.");

        RuleFor(p => p.ProductTypeValue)
            .NotEmpty().WithMessage("Ürün tipi boş olamaz.")
            .Must(BeAValidProductType).WithMessage("Geçersiz ürün tipi. Geçerli tipler: 'Mamul', 'Yarı Mamul'.");
    }

    private bool BeAValidProductType(string productTypeValue)
    {
        return ProductType.TryFromValue(productTypeValue, out _);
    }
}
