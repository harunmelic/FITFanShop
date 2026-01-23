using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Discounts.Queries.GetDiscountById;

public class GetDiscountByIdQueryHandler : IRequestHandler<GetDiscountByIdQuery, DiscountDetailsDto>
{
    private readonly IAppDbContext _ctx;

    public GetDiscountByIdQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<DiscountDetailsDto> Handle(GetDiscountByIdQuery request, CancellationToken cancellationToken)
    {
        var discount = await _ctx.Discounts
            .Include(d => d.DiscountProducts)
                .ThenInclude(dp => dp.Product)
            .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);

        if (discount == null)
            throw new FitFanShopNotFoundException($"Discount with id {request.Id} not found.");

        var products = discount.DiscountProducts
            .Where(dp => dp.Product != null && !dp.Product.IsDeleted)
            .Select(dp => new DiscountProductDto
            {
                ProductId = dp.ProductId,
                ProductName = dp.Product!.Name,
                ProductPrice = dp.Product.Price,
                DiscountedPrice = dp.Product.Price * (1 - discount.Percentage / 100)
            })
            .ToList();

        return new DiscountDetailsDto
        {
            Id = discount.Id,
            Name = discount.Name,
            Percentage = discount.Percentage,
            StartDate = discount.StartDate,
            EndDate = discount.EndDate,
            MembersOnly = discount.MembersOnly,
            IsActive = discount.IsActive,
            Products = products
        };
    }
}
