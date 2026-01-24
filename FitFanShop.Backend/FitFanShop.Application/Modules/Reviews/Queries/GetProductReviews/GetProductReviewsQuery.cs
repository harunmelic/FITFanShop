using FitFanShop.Application.Common;
using MediatR;

namespace FitFanShop.Application.Modules.Reviews.Queries.GetProductReviews;

public class GetProductReviewsQuery : BasePagedQuery<ReviewDto>
{
    public int ProductId { get; set; }
    public int? Rating { get; set; } // Filter by rating (1-5)
    public string? SortBy { get; set; } = "date_desc"; // date_desc, date_asc, rating_desc, rating_asc
}
