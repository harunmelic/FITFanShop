using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Catalog.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IAppDbContext _ctx;
    public GetProductByIdQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery query, CancellationToken ct)
    {
        var product = await _ctx.Products
            .Include(p => p.ProductCategories)
            .Include(p => p.Variants)
            .Include(p => p.Reviews)
            .Where(p => !p.IsDeleted)
            .FirstOrDefaultAsync(p => p.Id == query.Id && p.IsEnabled, ct); 

        if (product == null)
            throw new FitFanShopNotFoundException($"Product with id {query.Id} not found.");

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.Image,
            IsEnabled = product.IsEnabled,
            Exclusive = product.Exclusive,
            CategoryIds = product.ProductCategories
                .Where(pc => !pc.IsDeleted)
                .Select(pc => pc.CategoryId)
                .ToList(),
            Variants = product.Variants
                .Where(v => !v.IsDeleted)
                .Select(v => new ProductVariantDto
                {
                    Id = v.Id,
                    Size = v.Size,
                    StockQuantity = v.StockQuantity,
                    Sku = v.Sku
                })
                .ToList(),
            ReviewCount = product.Reviews.Count(r => !r.IsDeleted),
            AverageRating = product.Reviews.Any(r => !r.IsDeleted) 
                ? (decimal)Math.Round(product.Reviews.Where(r => !r.IsDeleted).Average(r => r.Rating), 2) 
                : 0
        };
    }
}
