using FitFanShop.Application.Modules.Catalog.Products;
using FitFanShop.Application.Modules.Catalog.Products.Commands.CreateProduct;
using FitFanShop.Application.Modules.Catalog.Products.Commands.UpdateProduct;
using FitFanShop.Application.Modules.Catalog.Products.Commands.DeleteProduct;
using FitFanShop.Application.Modules.Catalog.Products.Queries.GetAllProducts;
using FitFanShop.Application.Modules.Catalog.Products.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FitFanShop.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<ProductDto>>> GetAll([FromQuery] GetAllProductsQuery query)
        => Ok(await _mediator.Send(query));
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetById(int id)
        => Ok(await _mediator.Send(new GetProductByIdQuery(id)));
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductCommand command)
    {
        command.Id = id; 
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteProductCommand(id));
        return NoContent();
    }
}
