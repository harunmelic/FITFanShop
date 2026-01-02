namespace FitFanShop.Application.Common;
public abstract class BasePagedQuery<TItem> : IRequest<PageResult<TItem>>
{
    public PageRequest Paging { get; init; } = new();
}