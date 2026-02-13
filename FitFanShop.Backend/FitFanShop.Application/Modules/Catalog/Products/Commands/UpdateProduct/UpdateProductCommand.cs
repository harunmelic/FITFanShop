using MediatR;
using System.Text.Json.Serialization;
namespace FitFanShop.Application.Modules.Catalog.Products.Commands.UpdateProduct;

public class UpdateProductVariantDto
{
    public int? Id { get; set; }
    public string Size { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
}

public class UpdateProductCommand : IRequest<ProductDto>
{
    [JsonIgnore]
    public int Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public List<int>? CategoryIds { get; set; }
    public List<UpdateProductVariantDto>? Variants { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool Exclusive { get; set; } = false;
}
