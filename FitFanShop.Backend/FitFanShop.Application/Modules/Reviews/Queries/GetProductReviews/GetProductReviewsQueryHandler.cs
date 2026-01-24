using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Reviews.Queries.GetProductReviews;

public class GetProductReviewsQueryHandler : IRequestHandler<GetProductReviewsQuery, PageResult<ReviewDto>>
{
    private readonly IAppDbContext _ctx;

    public GetProductReviewsQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<PageResult<ReviewDto>> Handle(GetProductReviewsQuery request, CancellationToken cancellationToken)
    {
        var query = _ctx.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .Where(r => !r.IsDeleted && r.ProductId == request.ProductId)
            .AsQueryable();

        // Filter by rating
        if (request.Rating.HasValue)
            query = query.Where(r => r.Rating == request.Rating.Value);

        // Sorting
        query = request.SortBy?.ToLower() switch
        {
            "date_asc" => query.OrderBy(r => r.CreatedAtUtc),
            "rating_desc" => query.OrderByDescending(r => r.Rating).ThenByDescending(r => r.CreatedAtUtc),
            "rating_asc" => query.OrderBy(r => r.Rating).ThenByDescending(r => r.CreatedAtUtc),
            _ => query.OrderByDescending(r => r.CreatedAtUtc) // default: date_desc
        };

        var mappedQuery = query.Select(r => new ReviewDto
        {
            Id = r.Id,
            UserId = r.UserId,
            UserName = r.User != null ? $"{r.User.FirstName} {r.User.LastName}".Trim() : "Unknown",
            ProductId = r.ProductId,
            ProductName = r.Product != null ? r.Product.Name : "Unknown",
            OrderItemId = r.OrderItemId,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAtUtc
        });

        return await PageResult<ReviewDto>.FromQueryableAsync(mappedQuery, request.Paging, cancellationToken);
    }
}
