namespace FitFanShop.Application.Modules.Catalog.Products;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsEnabled { get; set; }
    public bool Exclusive { get; set; }
    public List<int> CategoryIds { get; set; } = new();
    public List<ProductVariantDto> Variants { get; set; } = new();
}
