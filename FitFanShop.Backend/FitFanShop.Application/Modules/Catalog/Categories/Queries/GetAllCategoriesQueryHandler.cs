using FitFanShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Catalog.Categories.Queries;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
{
    private readonly IAppDbContext _ctx;
    public GetAllCategoriesQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery query, CancellationToken ct)
    {
        var categoriesQuery = _ctx.Categories
            .Where(c => !c.IsDeleted && c.IsEnabled)
            .AsQueryable();

        var skip = (query.Page - 1) * query.PageSize;
        var categories = await categoriesQuery
            .Skip(skip)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            IsEnabled = c.IsEnabled
        }).ToList();
    }
}