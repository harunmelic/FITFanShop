using MediatR;

namespace FitFanShop.Application.Modules.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest
{
    public int UserId { get; set; }
}
