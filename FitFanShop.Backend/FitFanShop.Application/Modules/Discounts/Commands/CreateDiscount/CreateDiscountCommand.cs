using MediatR;

namespace FitFanShop.Application.Modules.Discounts.Commands.CreateDiscount;

public class CreateDiscountCommand : IRequest<DiscountDetailsDto>
{
    public required string Name { get; set; }
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool MembersOnly { get; set; }
    public List<int> ProductIds { get; set; } = new();
}
