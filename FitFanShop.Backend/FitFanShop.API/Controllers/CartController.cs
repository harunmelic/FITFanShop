using FitFanShop.Application.Modules.Commerce.Cart;
using FitFanShop.Application.Modules.Commerce.Cart.Commands.AddCartItem;
using FitFanShop.Application.Modules.Commerce.Cart.Commands.UpdateCartItem;
using FitFanShop.Application.Modules.Commerce.Cart.Commands.RemoveCartItem;
using FitFanShop.Application.Modules.Commerce.Cart.Commands.ClearCart;
using FitFanShop.Application.Modules.Commerce.Cart.Queries.GetCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFanShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddItem([FromBody] AddCartItemCommand command)
    {
        var cart = await _mediator.Send(command);
        return Ok(cart);
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var cart = await _mediator.Send(new GetCartQuery());
        return Ok(cart);
    }

    [HttpPut("items/{itemId}")]
    public async Task<ActionResult<CartDto>> UpdateItem(int itemId, [FromBody] UpdateCartItemDto dto)
    {
        var command = new UpdateCartItemCommand
        {
            ItemId = itemId,
            Quantity = dto.Quantity
        };
        var cart = await _mediator.Send(command);
        return Ok(cart);
    }

    [HttpDelete("items/{itemId}")]
    public async Task<ActionResult> RemoveItem(int itemId)
    {
        await _mediator.Send(new RemoveCartItemCommand { ItemId = itemId });
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> ClearCart()
    {
        await _mediator.Send(new ClearCartCommand());
        return NoContent();
    }
}
