using FitFanShop.Application.Common;
using FitFanShop.Application.Modules.Users;
using MediatR;

namespace FitFanShop.Application.Modules.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<PageResult<UserDto>>
{
    public PageRequest Paging { get; set; } = new();
    public string? Search { get; set; }
}
