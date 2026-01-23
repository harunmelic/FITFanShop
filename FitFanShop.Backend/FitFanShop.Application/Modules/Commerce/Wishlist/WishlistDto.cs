namespace FitFanShop.Application.Modules.Commerce.Wishlist;

public class WishlistDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<WishlistItemDto> Items { get; set; } = new();
    public int ItemCount { get; set; }
}
