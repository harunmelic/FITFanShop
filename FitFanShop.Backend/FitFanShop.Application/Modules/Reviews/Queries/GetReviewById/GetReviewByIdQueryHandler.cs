using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Reviews.Queries.GetReviewById;

public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, ReviewDto>
{
    private readonly IAppDbContext _ctx;

    public GetReviewByIdQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ReviewDto> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        var review = await _ctx.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .Where(r => !r.IsDeleted)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (review == null)
            throw new FitFanShopNotFoundException($"Review with id {request.Id} not found.");

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
