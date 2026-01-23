using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using FitFanShop.Domain.Entities.Discounts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Discounts.Commands.UpdateDiscount;

public class UpdateDiscountCommandHandler : IRequestHandler<UpdateDiscountCommand, DiscountDetailsDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IMediator _mediator;

    public UpdateDiscountCommandHandler(IAppDbContext ctx, IMediator mediator)
    {
        _ctx = ctx;
        _mediator = mediator;
    }

    public async Task<DiscountDetailsDto> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
    {
        var discount = await _ctx.Discounts
            .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);

        if (discount == null)
            throw new FitFanShopNotFoundException($"Discount with id {request.Id} not found.");

        discount.Name = request.Name.Trim();
        discount.Percentage = request.Percentage;
        discount.StartDate = request.StartDate;
        discount.EndDate = request.EndDate;
        discount.MembersOnly = request.MembersOnly;
        discount.ModifiedAtUtc = DateTime.UtcNow;

        var existingProducts = await _ctx.DiscountProducts
            .Where(dp => dp.DiscountId == request.Id)
            .ToListAsync(cancellationToken);

        _ctx.DiscountProducts.RemoveRange(existingProducts);

        if (request.ProductIds.Any())
        {
            var newDiscountProducts = request.ProductIds
                .Distinct()
                .Select(productId => new DiscountProductEntity
                {
                    DiscountId = discount.Id,
                    ProductId = productId,
                    CreatedAtUtc = DateTime.UtcNow
                })
                .ToList();

            _ctx.DiscountProducts.AddRange(newDiscountProducts);
        }

        await _ctx.SaveChangesAsync(cancellationToken);

        return await _mediator.Send(new Queries.GetDiscountById.GetDiscountByIdQuery { Id = discount.Id }, cancellationToken);
    }
}
