using MediatR;
using System.Text.Json.Serialization;
namespace FitFanShop.Application.Modules.Catalog.Products.Commands.UpdateProduct;
public class UpdateProductCommand : IRequest<ProductDto>
{
    [JsonIgnore]
    public int Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public List<int>? CategoryIds { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool Exclusive { get; set; } = false;
}
