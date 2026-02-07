using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using FitFanShop.Domain.Entities.Reviews;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public CreateReviewCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        // Check if OrderItem exists and belongs to current user
        var orderItem = await _ctx.OrderItems
            .Include(oi => oi.Order)
            .Include(oi => oi.ProductVariant)
                .ThenInclude(pv => pv.Product)
            .Include(oi => oi.Review)
            .FirstOrDefaultAsync(oi => oi.Id == request.OrderItemId && !oi.IsDeleted, cancellationToken);

        if (orderItem == null)
            throw new FitFanShopNotFoundException($"Order item with id {request.OrderItemId} not found.");

        if (orderItem.Order == null || orderItem.Order.UserId != userId.Value)
            throw new FitFanShopBusinessRuleException("Unauthorized", "Možete recenzirati samo proizvode koje ste kupili.");

        // Check if review already exists for this OrderItem
        if (orderItem.Review != null)
            throw new FitFanShopBusinessRuleException("ReviewAlreadyExists", "Već ste recenzirali ovaj proizvod.");

        // Check if the order is confirmed or delivered
        var order = await _ctx.Orders
            .Include(o => o.Status)
            .FirstOrDefaultAsync(o => o.Id == orderItem.OrderId, cancellationToken);

        if (order == null || order.Status == null)
            throw new FitFanShopNotFoundException("Order not found.");

        var allowedStatuses = new[] { "Confirmed", "Delivered" };
        if (!allowedStatuses.Contains(order.Status.Name))
            throw new FitFanShopBusinessRuleException("OrderNotConfirmedOrDelivered", 
                "Možete recenzirati samo proizvode iz potvrđenih ili isporučenih narudžbi.");

        // Get ProductId from ProductVariant
        if (orderItem.ProductVariant == null)
            throw new FitFanShopNotFoundException("Product variant not found.");

        var productId = orderItem.ProductVariant.ProductId;

        // Create Review
        var review = new ReviewEntity
        {
            UserId = userId.Value,
            ProductId = productId,
            OrderItemId = request.OrderItemId,
            Rating = request.Rating,
            Comment = request.Comment?.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        _ctx.Reviews.Add(review);
        await _ctx.SaveChangesAsync(cancellationToken);

        // Fetch created review with relationships
        var createdReview = await _ctx.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == review.Id, cancellationToken);

        return new ReviewDto
        {
            Id = createdReview!.Id,
            UserId = createdReview.UserId,
            UserName = $"{createdReview.User?.FirstName} {createdReview.User?.LastName}".Trim(),
            ProductId = createdReview.ProductId,
            ProductName = createdReview.Product?.Name ?? "Unknown",
            OrderItemId = createdReview.OrderItemId,
            Rating = createdReview.Rating,
            Comment = createdReview.Comment,
            CreatedAt = createdReview.CreatedAtUtc
        };
    }
}
