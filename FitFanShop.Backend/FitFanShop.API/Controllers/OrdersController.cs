using FitFanShop.Application.Common;
using FitFanShop.Application.Modules.Sales.Orders;
using FitFanShop.Application.Modules.Sales.Orders.Commands.CreateOrder;
using FitFanShop.Application.Modules.Sales.Orders.Commands.CancelOrder;
using FitFanShop.Application.Modules.Sales.Orders.Commands.DeleteOrder;
using FitFanShop.Application.Modules.Sales.Orders.Commands.UpdateOrderStatus;
using FitFanShop.Application.Modules.Sales.Orders.Queries.GetOrderById;
using FitFanShop.Application.Modules.Sales.Orders.Queries.GetMyOrders;
using FitFanShop.Application.Modules.Orders.Queries.CanUserReviewProduct;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFanShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateOrderCommand command)
    {
        var orderId = await _mediator.Send(command);
        return Ok(orderId);
    }

    [HttpGet("my-orders")]
    public async Task<ActionResult<PageResult<OrderDto>>> GetMyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetMyOrdersQuery
        {
            Paging = new PageRequest { Page = page, PageSize = pageSize }
        };
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery { Id = id });
        return Ok(order);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteOrderCommand { OrderId = id });
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        await _mediator.Send(new UpdateOrderStatusCommand { OrderId = id, Status = dto.Status });
        return NoContent();
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> Cancel(int id)
    {
        await _mediator.Send(new CancelOrderCommand { OrderId = id });
        return NoContent();
    }

    [HttpGet("can-review/{productId}")]
    public async Task<ActionResult<CanUserReviewProductDto>> CanReviewProduct(int productId)
    {
        var result = await _mediator.Send(new CanUserReviewProductQuery { ProductId = productId });
        return Ok(result);
    }
}
