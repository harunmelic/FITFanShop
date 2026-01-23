using FitFanShop.Application.Modules.Users;
using MediatR;

namespace FitFanShop.Application.Modules.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<UserDto>
{
    public int Id { get; set; }
}
