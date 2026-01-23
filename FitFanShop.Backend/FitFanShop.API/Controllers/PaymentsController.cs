using FitFanShop.Application.Modules.Payments.Commands.MockCheckout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFanShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<MockCheckoutResultDto>> MockCheckout()
    {
        var result = await _mediator.Send(new MockCheckoutCommand());
        return Ok(result);
    }
}
