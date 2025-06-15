using ERP.Server.Domain.Entities.Enums;
using FluentValidation;

namespace ERP.Server.Application.Features.Products.Commands;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(p => p.ProductTypeValue)
            .NotEmpty()
            .Must(v => ProductType.TryFromValue(v, out _))
            .WithMessage("Invalid product type");
    }
}
