using MediatR;

namespace FitFanShop.Application.Modules.Catalog.Products.Commands.CreateProduct;

public class CreateProductVariantDto
{
    public string Size { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
}

public class CreateProductCommand : IRequest<ProductDto>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public List<int> CategoryIds { get; set; } = new();
    public bool IsEnabled { get; set; } = true;
    public bool Exclusive { get; set; } = false;
    public List<CreateProductVariantDto> Variants { get; set; } = new();
}
