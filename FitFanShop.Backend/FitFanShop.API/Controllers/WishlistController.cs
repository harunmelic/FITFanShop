using FitFanShop.Application.Modules.Commerce.Wishlist;
using FitFanShop.Application.Modules.Commerce.Wishlist.Commands.AddToWishlist;
using FitFanShop.Application.Modules.Commerce.Wishlist.Commands.RemoveFromWishlist;
using FitFanShop.Application.Modules.Commerce.Wishlist.Commands.ClearWishlist;
using FitFanShop.Application.Modules.Commerce.Wishlist.Queries.GetWishlist;
using FitFanShop.Application.Modules.Commerce.Wishlist.Queries.CheckProductInWishlist;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFanShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IMediator _mediator;

    public WishlistController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("items")]
    public async Task<ActionResult<WishlistDto>> AddProduct([FromBody] AddToWishlistCommand command)
    {
        var wishlist = await _mediator.Send(command);
        return Ok(wishlist);
    }

    [HttpGet]
    public async Task<ActionResult<WishlistDto>> GetWishlist()
    {
        var wishlist = await _mediator.Send(new GetWishlistQuery());
        return Ok(wishlist);
    }

    [HttpGet("check/{productId}")]
    public async Task<ActionResult<CheckProductInWishlistDto>> CheckProduct(int productId)
    {
        var result = await _mediator.Send(new CheckProductInWishlistQuery { ProductId = productId });
        return Ok(result);
    }

    [HttpDelete("items/{itemId}")]
    public async Task<ActionResult> RemoveItem(int itemId)
    {
        await _mediator.Send(new RemoveFromWishlistCommand { ItemId = itemId });
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> ClearWishlist()
    {
        await _mediator.Send(new ClearWishlistCommand());
        return NoContent();
    }
}
