using System.ComponentModel;
using MediatR;

namespace FitFanShop.Application.Modules.Catalog.Products.Queries.GetAllProducts;

public class GetAllProductsQuery : IRequest<List<ProductDto>>
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsExclusive { get; set; }
    [DefaultValue(1)]
    public int Page { get; set; } = 1;
    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;
    public string? Sort { get; set; }
}
