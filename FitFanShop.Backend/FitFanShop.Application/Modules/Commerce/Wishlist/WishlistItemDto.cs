namespace FitFanShop.Application.Modules.Commerce.Wishlist;

public class WishlistItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsExclusive { get; set; }
    public DateTime AddedAt { get; set; }
}
