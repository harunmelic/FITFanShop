using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Reviews.Commands.UpdateReview;

public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, ReviewDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public UpdateReviewCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<ReviewDto> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var review = await _ctx.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

        if (review == null)
            throw new FitFanShopNotFoundException($"Review with id {request.Id} not found.");

        // Only the owner can update their review
        if (review.UserId != userId.Value)
            throw new FitFanShopBusinessRuleException("Unauthorized", "You can only update your own reviews.");

        // Update review
        review.Rating = request.Rating;
        review.Comment = request.Comment?.Trim();
        review.ModifiedAtUtc = DateTime.UtcNow;

        await _ctx.SaveChangesAsync(cancellationToken);

        return new ReviewDto
        {
            Id = review.Id,
            UserId = review.UserId,
            UserName = $"{review.User?.FirstName} {review.User?.LastName}".Trim(),
            ProductId = review.ProductId,
            ProductName = review.Product?.Name ?? "Unknown",
            OrderItemId = review.OrderItemId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAtUtc
        };
    }
}
