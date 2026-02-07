namespace FitFanShop.Application.Modules.Orders.Queries.CanUserReviewProduct;

public class CanUserReviewProductDto
{
    public bool CanReview { get; set; }
    public int? OrderItemId { get; set; }
    public bool HasAlreadyReviewed { get; set; }
    public string? Reason { get; set; }
}
