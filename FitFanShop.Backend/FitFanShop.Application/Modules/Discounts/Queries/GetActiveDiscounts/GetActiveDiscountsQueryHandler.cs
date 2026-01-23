using FitFanShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Discounts.Queries.GetActiveDiscounts;

public class GetActiveDiscountsQueryHandler : IRequestHandler<GetActiveDiscountsQuery, List<DiscountDto>>
{
    private readonly IAppDbContext _ctx;

    public GetActiveDiscountsQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<DiscountDto>> Handle(GetActiveDiscountsQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        return await _ctx.Discounts
            .Include(d => d.DiscountProducts)
            .Where(d => !d.IsDeleted && d.StartDate <= now && d.EndDate >= now)
            .OrderByDescending(d => d.Percentage)
            .Select(d => new DiscountDto
            {
                Id = d.Id,
                Name = d.Name,
                Percentage = d.Percentage,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                MembersOnly = d.MembersOnly,
                IsActive = true,
                ProductCount = d.DiscountProducts.Count
            })
            .ToListAsync(cancellationToken);
    }
}
