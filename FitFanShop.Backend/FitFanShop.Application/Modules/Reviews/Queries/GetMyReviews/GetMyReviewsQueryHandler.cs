using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Reviews.Queries.GetMyReviews;

public class GetMyReviewsQueryHandler : IRequestHandler<GetMyReviewsQuery, PageResult<ReviewDto>>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public GetMyReviewsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<PageResult<ReviewDto>> Handle(GetMyReviewsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var query = _ctx.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .Where(r => !r.IsDeleted && r.UserId == userId.Value)
            .AsQueryable();

        // Filter by product
        if (request.ProductId.HasValue)
            query = query.Where(r => r.ProductId == request.ProductId.Value);

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
