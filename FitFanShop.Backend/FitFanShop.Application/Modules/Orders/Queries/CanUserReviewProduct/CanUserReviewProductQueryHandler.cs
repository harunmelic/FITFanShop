using FitFanShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Orders.Queries.CanUserReviewProduct;

public class CanUserReviewProductQueryHandler : IRequestHandler<CanUserReviewProductQuery, CanUserReviewProductDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public CanUserReviewProductQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<CanUserReviewProductDto> Handle(CanUserReviewProductQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        
        if (userId == null)
        {
            return new CanUserReviewProductDto
            {
                CanReview = false,
                Reason = "Morate biti prijavljeni da biste napisali recenziju"
            };
        }

        // Find if user has purchased this product (via ProductVariant -> OrderItem)
        var orderItem = await _ctx.OrderItems
            .Include(oi => oi.Order)
                .ThenInclude(o => o.Status)
            .Include(oi => oi.ProductVariant)
            .Include(oi => oi.Review)
            .Where(oi => 
                !oi.IsDeleted &&
                oi.Order != null &&
                oi.Order.UserId == userId.Value &&
                oi.ProductVariant != null &&
                oi.ProductVariant.ProductId == request.ProductId)
            .OrderByDescending(oi => oi.Order!.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (orderItem == null)
        {
            return new CanUserReviewProductDto
            {
                CanReview = false,
                Reason = "Morate kupiti ovaj proizvod prije nego ga možete recenzirati"
            };
        }

        // Check if already reviewed
        if (orderItem.Review != null)
        {
            return new CanUserReviewProductDto
            {
                CanReview = false,
                HasAlreadyReviewed = true,
                Reason = "Već ste recenzirali ovaj proizvod"
            };
        }

        // Check if order is confirmed or delivered
        var allowedStatuses = new[] { "Confirmed", "Delivered" };
        if (orderItem.Order?.Status == null || !allowedStatuses.Contains(orderItem.Order.Status.Name))
        {
            return new CanUserReviewProductDto
            {
                CanReview = false,
                Reason = "Možete recenzirati samo proizvode iz potvrđenih ili isporučenih narudžbi"
            };
        }

        return new CanUserReviewProductDto
        {
            CanReview = true,
            OrderItemId = orderItem.Id,
            HasAlreadyReviewed = false
        };
    }
}
