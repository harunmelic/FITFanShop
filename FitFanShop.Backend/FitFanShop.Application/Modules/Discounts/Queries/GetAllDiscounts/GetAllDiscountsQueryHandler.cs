using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Discounts.Queries.GetAllDiscounts;

public class GetAllDiscountsQueryHandler : IRequestHandler<GetAllDiscountsQuery, PageResult<DiscountDto>>
{
    private readonly IAppDbContext _ctx;

    public GetAllDiscountsQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<PageResult<DiscountDto>> Handle(GetAllDiscountsQuery request, CancellationToken cancellationToken)
    {
        var query = _ctx.Discounts
            .Include(d => d.DiscountProducts)
            .Where(d => !d.IsDeleted)
            .AsQueryable();

        if (request.ActiveOnly == true)
        {
            var now = DateTime.UtcNow;
            query = query.Where(d => d.StartDate <= now && d.EndDate >= now);
        }

        if (request.MembersOnly.HasValue)
        {
            query = query.Where(d => d.MembersOnly == request.MembersOnly.Value);
        }

        var total = await query.CountAsync(cancellationToken);

        var discounts = await query
            .OrderByDescending(d => d.CreatedAtUtc)
            .Skip((request.Paging.Page - 1) * request.Paging.PageSize)
            .Take(request.Paging.PageSize)
            .Select(d => new DiscountDto
            {
                Id = d.Id,
                Name = d.Name,
                Percentage = d.Percentage,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                MembersOnly = d.MembersOnly,
                IsActive = d.IsActive,
                ProductCount = d.DiscountProducts.Count
            })
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(total / (double)request.Paging.PageSize);

        return new PageResult<DiscountDto>
        {
            Items = discounts,
            TotalItems = total,
            CurrentPage = request.Paging.Page,
            PageSize = request.Paging.PageSize,
            IncludedTotal = true,
            TotalPages = totalPages
        };
    }
}
