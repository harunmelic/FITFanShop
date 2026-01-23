namespace FitFanShop.Application.Modules.Discounts;

public class DiscountDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool MembersOnly { get; set; }
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
}
