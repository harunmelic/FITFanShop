namespace FitFanShop.Application.Modules.Catalog.Products;

public class ProductVariantDto
{
    public int Id { get; set; }
    public string Size { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public string Sku { get; set; } = string.Empty;
}
