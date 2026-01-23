using MediatR;

namespace FitFanShop.Application.Modules.ActivityLogs.Queries.GetUserActivityLogs;

public class GetUserActivityLogsQuery : IRequest<List<ActivityLogDto>>
{
    public int UserId { get; set; }
}
