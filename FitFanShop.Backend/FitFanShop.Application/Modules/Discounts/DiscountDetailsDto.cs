namespace FitFanShop.Application.Modules.Discounts;

public class DiscountDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool MembersOnly { get; set; }
    public bool IsActive { get; set; }
    public List<DiscountProductDto> Products { get; set; } = new();
}

public class DiscountProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
}
