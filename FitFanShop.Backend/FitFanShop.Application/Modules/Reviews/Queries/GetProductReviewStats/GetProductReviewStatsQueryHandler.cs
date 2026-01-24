using FitFanShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Reviews.Queries.GetProductReviewStats;

public class GetProductReviewStatsQueryHandler : IRequestHandler<GetProductReviewStatsQuery, ProductReviewStatsDto>
{
    private readonly IAppDbContext _ctx;

    public GetProductReviewStatsQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ProductReviewStatsDto> Handle(GetProductReviewStatsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _ctx.Reviews
            .Where(r => !r.IsDeleted && r.ProductId == request.ProductId)
            .ToListAsync(cancellationToken);

        var totalReviews = reviews.Count;
        var averageRating = totalReviews > 0 ? reviews.Average(r => r.Rating) : 0;

        return new ProductReviewStatsDto
        {
            ProductId = request.ProductId,
            TotalReviews = totalReviews,
            AverageRating = (decimal)Math.Round(averageRating, 2),
            FiveStarCount = reviews.Count(r => r.Rating == 5),
            FourStarCount = reviews.Count(r => r.Rating == 4),
            ThreeStarCount = reviews.Count(r => r.Rating == 3),
            TwoStarCount = reviews.Count(r => r.Rating == 2),
            OneStarCount = reviews.Count(r => r.Rating == 1)
        };
    }
}
