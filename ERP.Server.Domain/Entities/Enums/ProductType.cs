using Ardalis.SmartEnum;

namespace ERP.Server.Domain.Entities.Enums;

public sealed class ProductType : SmartEnum<ProductType, string>
{
    public static readonly ProductType SemiFinished = new("Yarı Mamul", "Yarı Mamul");
    public static readonly ProductType Finished = new("Mamul", "Mamul");

    private ProductType(string name, string value) : base(name, value)
    {
    }
}
