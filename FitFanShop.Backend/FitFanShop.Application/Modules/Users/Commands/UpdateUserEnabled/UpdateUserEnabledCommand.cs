using MediatR;

namespace FitFanShop.Application.Modules.Users.Commands.UpdateUserEnabled;

public class UpdateUserEnabledCommand : IRequest
{
    public int UserId { get; set; }
    public bool IsEnabled { get; set; }
}
