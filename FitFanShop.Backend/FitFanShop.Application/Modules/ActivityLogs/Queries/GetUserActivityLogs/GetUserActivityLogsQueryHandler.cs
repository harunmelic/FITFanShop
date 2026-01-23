using FitFanShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.ActivityLogs.Queries.GetUserActivityLogs;

public class GetUserActivityLogsQueryHandler : IRequestHandler<GetUserActivityLogsQuery, List<ActivityLogDto>>
{
    private readonly IAppDbContext _ctx;

    public GetUserActivityLogsQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<ActivityLogDto>> Handle(GetUserActivityLogsQuery request, CancellationToken cancellationToken)
    {
        return await _ctx.ActivityLogs
            .Include(a => a.User)
            .Where(a => a.UserId == request.UserId && !a.IsDeleted)
            .OrderByDescending(a => a.CreatedAtUtc)
            .Take(50)
            .Select(a => new ActivityLogDto
            {
                Id = a.Id,
                UserId = a.UserId,
                UserEmail = a.User != null ? a.User.Email : "Unknown",
                ActionDescription = a.ActionDescription,
                ActionDate = a.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }
}
