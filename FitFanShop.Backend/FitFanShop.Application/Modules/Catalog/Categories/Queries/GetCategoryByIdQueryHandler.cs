using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Catalog.Categories.Queries;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly IAppDbContext _ctx;
    public GetCategoryByIdQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery query, CancellationToken ct)
    {
        var category = await _ctx.Categories.FirstOrDefaultAsync(c => c.Id == query.Id && !c.IsDeleted, ct);
        if (category == null)
            throw new FitFanShopNotFoundException($"Category with id {query.Id} not found.");
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsEnabled = category.IsEnabled
        };
    }
}
