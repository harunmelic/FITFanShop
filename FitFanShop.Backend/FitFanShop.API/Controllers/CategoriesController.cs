using FitFanShop.Application.Modules.Catalog.Categories;
using FitFanShop.Application.Modules.Catalog.Categories.Commands;
using FitFanShop.Application.Modules.Catalog.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FitFanShop.API.Controllers;
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    public CategoriesController(IMediator mediator) => _mediator = mediator;
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<CategoryDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetAllCategoriesQuery { Page = page, PageSize = pageSize };
        return Ok(await _mediator.Send(query));
    }
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
        => Ok(await _mediator.Send(new GetCategoryByIdQuery(id)));
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] UpdateCategoryCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteCategoryCommand(id));
        return NoContent();
    }
    [HttpPatch("{id}/enabled")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetEnabled(int id, [FromBody] SetCategoryEnabledDto dto)
    {
        await _mediator.Send(new SetCategoryEnabledCommand { Id = id, IsEnabled = dto.IsEnabled });
        return NoContent();
    }
}
