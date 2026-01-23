using FitFanShop.Application.Abstractions;
using FitFanShop.Domain.Entities.Discounts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Discounts.Commands.CreateDiscount;

public class CreateDiscountCommandHandler : IRequestHandler<CreateDiscountCommand, DiscountDetailsDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IMediator _mediator;

    public CreateDiscountCommandHandler(IAppDbContext ctx, IMediator mediator)
    {
        _ctx = ctx;
        _mediator = mediator;
    }

    public async Task<DiscountDetailsDto> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
    {
        var discount = new DiscountEntity
        {
            Name = request.Name.Trim(),
            Percentage = request.Percentage,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            MembersOnly = request.MembersOnly,
            CreatedAtUtc = DateTime.UtcNow
        };

        _ctx.Discounts.Add(discount);
        await _ctx.SaveChangesAsync(cancellationToken);

        if (request.ProductIds.Any())
        {
            var discountProducts = request.ProductIds
                .Distinct()
                .Select(productId => new DiscountProductEntity
                {
                    DiscountId = discount.Id,
                    ProductId = productId,
                    CreatedAtUtc = DateTime.UtcNow
                })
                .ToList();

            _ctx.DiscountProducts.AddRange(discountProducts);
            await _ctx.SaveChangesAsync(cancellationToken);
        }

        return await _mediator.Send(new Queries.GetDiscountById.GetDiscountByIdQuery { Id = discount.Id }, cancellationToken);
    }
}
