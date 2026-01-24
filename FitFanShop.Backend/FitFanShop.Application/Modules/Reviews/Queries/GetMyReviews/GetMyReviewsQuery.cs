using FitFanShop.Application.Common;
using MediatR;

namespace FitFanShop.Application.Modules.Reviews.Queries.GetMyReviews;

public class GetMyReviewsQuery : BasePagedQuery<ReviewDto>
{
    public int? ProductId { get; set; } // Filter by specific product
    public string? SortBy { get; set; } = "date_desc"; // date_desc, date_asc, rating_desc, rating_asc
}
