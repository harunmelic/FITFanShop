using MediatR;
using System.Text.Json.Serialization;

namespace FitFanShop.Application.Modules.Discounts.Commands.UpdateDiscount;

public class UpdateDiscountCommand : IRequest<DiscountDetailsDto>
{
    [JsonIgnore]
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool MembersOnly { get; set; }
    public List<int> ProductIds { get; set; } = new();
}
