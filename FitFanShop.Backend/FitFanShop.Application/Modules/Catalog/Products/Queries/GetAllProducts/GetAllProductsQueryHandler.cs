using FitFanShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace FitFanShop.Application.Modules.Catalog.Products.Queries.GetAllProducts;
public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly IAppDbContext _ctx;
    public GetAllProductsQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }
    public async Task<List<ProductDto>> Handle(GetAllProductsQuery query, CancellationToken ct)
    {
        var productsQuery = _ctx.Products
            .Include(p => p.ProductCategories)
            .Include(p => p.Variants)
            .Include(p => p.Reviews)
            .Where(p => !p.IsDeleted && p.IsEnabled) 
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Search))
            productsQuery = productsQuery.Where(p => p.Name.Contains(query.Search));
        if (query.CategoryId.HasValue)
            productsQuery = productsQuery.Where(p => p.ProductCategories.Any(pc => !pc.IsDeleted && pc.CategoryId == query.CategoryId));
        if (query.MinPrice.HasValue)
            productsQuery = productsQuery.Where(p => p.Price >= query.MinPrice);
        if (query.MaxPrice.HasValue)
            productsQuery = productsQuery.Where(p => p.Price <= query.MaxPrice);
        if (query.IsExclusive.HasValue)
            productsQuery = productsQuery.Where(p => p.Exclusive == query.IsExclusive);
        if (!string.IsNullOrWhiteSpace(query.Sort))
        {
            if (query.Sort == "price_asc")
                productsQuery = productsQuery.OrderBy(p => p.Price);
            else if (query.Sort == "price_desc")
                productsQuery = productsQuery.OrderByDescending(p => p.Price);
            else if (query.Sort == "name")
                productsQuery = productsQuery.OrderBy(p => p.Name);
        }
        else
        {
            productsQuery = productsQuery.OrderBy(p => p.Id);
        }
        var skip = (query.Page - 1) * query.PageSize;
        var products = await productsQuery
            .Skip(skip)
            .Take(query.PageSize)
            .ToListAsync(ct);
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            IsEnabled = p.IsEnabled,
            Exclusive = p.Exclusive,
            CategoryIds = p.ProductCategories
                .Where(pc => !pc.IsDeleted)
                .Select(pc => pc.CategoryId)
                .ToList(),
            Variants = p.Variants
                .Where(v => !v.IsDeleted)
                .Select(v => new ProductVariantDto
                {
                    Id = v.Id,
                    Size = v.Size,
                    StockQuantity = v.StockQuantity,
                    Sku = v.Sku
                })
                .ToList(),
            ReviewCount = p.Reviews.Count(r => !r.IsDeleted),
            AverageRating = p.Reviews.Any(r => !r.IsDeleted) 
                ? (decimal)Math.Round(p.Reviews.Where(r => !r.IsDeleted).Average(r => r.Rating), 2) 
                : 0
        }).ToList();
    }
}
