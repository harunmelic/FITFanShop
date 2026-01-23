using MediatR;

namespace FitFanShop.Application.Modules.Payments.Commands.MockCheckout;

public class MockCheckoutCommand : IRequest<MockCheckoutResultDto>
{
}

public class MockCheckoutResultDto
{
    public int OrderId { get; set; }
    public int PaymentId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Message { get; set; } = "Payment successful! Order created.";
}
