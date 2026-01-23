using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.ActivityLogs.Queries.GetAllActivityLogs;

public class GetAllActivityLogsQueryHandler : IRequestHandler<GetAllActivityLogsQuery, PageResult<ActivityLogDto>>
{
    private readonly IAppDbContext _ctx;

    public GetAllActivityLogsQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<PageResult<ActivityLogDto>> Handle(GetAllActivityLogsQuery request, CancellationToken cancellationToken)
    {
        // Default date range: last 30 days if not specified
        var fromDate = request.FromDate ?? DateTime.UtcNow.AddDays(-30);
        var toDate = request.ToDate ?? DateTime.UtcNow;

        var query = _ctx.ActivityLogs
            .Include(a => a.User)
            .Where(a => !a.IsDeleted)
            .Where(a => a.CreatedAtUtc >= fromDate && a.CreatedAtUtc <= toDate)
            .AsQueryable();

        var total = await query.CountAsync(cancellationToken);

        var logs = await query
            .OrderByDescending(a => a.CreatedAtUtc)
            .Skip((request.Paging.Page - 1) * request.Paging.PageSize)
            .Take(request.Paging.PageSize)
            .Select(a => new ActivityLogDto
            {
                Id = a.Id,
                UserId = a.UserId,
                UserEmail = a.User != null ? a.User.Email : "Unknown",
                ActionDescription = a.ActionDescription,
                ActionDate = a.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(total / (double)request.Paging.PageSize);

        return new PageResult<ActivityLogDto>
        {
            Items = logs,
            TotalItems = total,
            TotalPages = totalPages,
            CurrentPage = request.Paging.Page,
            PageSize = request.Paging.PageSize,
            IncludedTotal = true
        };
    }
}
