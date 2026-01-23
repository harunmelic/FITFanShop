using MediatR;

namespace FitFanShop.Application.Modules.Discounts.Commands.DeleteDiscount;

public class DeleteDiscountCommand : IRequest
{
    public int Id { get; set; }
}
