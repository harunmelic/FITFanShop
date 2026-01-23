using FitFanShop.Application.Common;
using FitFanShop.Application.Modules.ActivityLogs;
using FitFanShop.Application.Modules.ActivityLogs.Queries.GetAllActivityLogs;
using FitFanShop.Application.Modules.ActivityLogs.Queries.GetUserActivityLogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFanShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ActivityLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ActivityLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<ActivityLogDto>>> GetAllActivityLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var query = new GetAllActivityLogsQuery
        {
            Paging = new PageRequest { Page = page, PageSize = pageSize },
            FromDate = fromDate,
            ToDate = toDate
        };

        var logs = await _mediator.Send(query);
        return Ok(logs);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<ActivityLogDto>>> GetUserActivityLogs(int userId)
    {
        var logs = await _mediator.Send(new GetUserActivityLogsQuery { UserId = userId });
        return Ok(logs);
    }
}
