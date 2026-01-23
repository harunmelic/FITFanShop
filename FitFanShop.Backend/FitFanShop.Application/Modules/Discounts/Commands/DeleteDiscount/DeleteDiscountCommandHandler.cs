using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Discounts.Commands.DeleteDiscount;

public class DeleteDiscountCommandHandler : IRequestHandler<DeleteDiscountCommand>
{
    private readonly IAppDbContext _ctx;

    public DeleteDiscountCommandHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
    {
        var discount = await _ctx.Discounts
            .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);

        if (discount == null)
            throw new FitFanShopNotFoundException($"Discount with id {request.Id} not found.");

        discount.IsDeleted = true;
        discount.ModifiedAtUtc = DateTime.UtcNow;

        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
