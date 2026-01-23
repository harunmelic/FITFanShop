namespace FitFanShop.Application.Modules.Commerce.Cart;

public class CartDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
}
