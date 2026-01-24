namespace FitFanShop.Application.Modules.Reviews;

public class ProductReviewStatsDto
{
    public int ProductId { get; set; }
    public int TotalReviews { get; set; }
    public decimal AverageRating { get; set; }
    public int FiveStarCount { get; set; }
    public int FourStarCount { get; set; }
    public int ThreeStarCount { get; set; }
    public int TwoStarCount { get; set; }
    public int OneStarCount { get; set; }
}
