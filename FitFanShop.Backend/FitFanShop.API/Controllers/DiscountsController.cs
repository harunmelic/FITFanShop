using FitFanShop.Application.Common;
using FitFanShop.Application.Modules.Discounts;
using FitFanShop.Application.Modules.Discounts.Commands.CreateDiscount;
using FitFanShop.Application.Modules.Discounts.Commands.DeleteDiscount;
using FitFanShop.Application.Modules.Discounts.Commands.UpdateDiscount;
using FitFanShop.Application.Modules.Discounts.Queries.GetActiveDiscounts;
using FitFanShop.Application.Modules.Discounts.Queries.GetAllDiscounts;
using FitFanShop.Application.Modules.Discounts.Queries.GetDiscountById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFanShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DiscountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DiscountDetailsDto>> CreateDiscount([FromBody] CreateDiscountCommand command)
    {
        var discount = await _mediator.Send(command);
        return Ok(discount);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PageResult<DiscountDto>>> GetAllDiscounts(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? activeOnly = null,
        [FromQuery] bool? membersOnly = null)
    {
        var query = new GetAllDiscountsQuery
        {
            Paging = new PageRequest { Page = page, PageSize = pageSize },
            ActiveOnly = activeOnly,
            MembersOnly = membersOnly
        };
        var discounts = await _mediator.Send(query);
        return Ok(discounts);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult<List<DiscountDto>>> GetActiveDiscounts()
    {
        var discounts = await _mediator.Send(new GetActiveDiscountsQuery());
        return Ok(discounts);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DiscountDetailsDto>> GetDiscountById(int id)
    {
        var discount = await _mediator.Send(new GetDiscountByIdQuery { Id = id });
        return Ok(discount);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DiscountDetailsDto>> UpdateDiscount(int id, [FromBody] UpdateDiscountCommand command)
    {
        command.Id = id;
        var discount = await _mediator.Send(command);
        return Ok(discount);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteDiscount(int id)
    {
        await _mediator.Send(new DeleteDiscountCommand { Id = id });
        return NoContent();
    }
}
