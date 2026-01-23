using FitFanShop.Application.Common;
using MediatR;

namespace FitFanShop.Application.Modules.ActivityLogs.Queries.GetAllActivityLogs;

public class GetAllActivityLogsQuery : IRequest<PageResult<ActivityLogDto>>
{
    public PageRequest Paging { get; set; } = new();
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
