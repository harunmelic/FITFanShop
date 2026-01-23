using FitFanShop.Application.Common;
using FitFanShop.Application.Modules.Users;
using FitFanShop.Application.Modules.Users.Commands.ChangePassword;
using FitFanShop.Application.Modules.Users.Commands.DeleteMyAccount;
using FitFanShop.Application.Modules.Users.Commands.DeleteUser;
using FitFanShop.Application.Modules.Users.Commands.UpdateMyProfile;
using FitFanShop.Application.Modules.Users.Commands.UpdateUserEnabled;
using FitFanShop.Application.Modules.Users.Queries.GetAllUsers;
using FitFanShop.Application.Modules.Users.Queries.GetMyProfile;
using FitFanShop.Application.Modules.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFanShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetMyProfile()
    {
        var profile = await _mediator.Send(new GetMyProfileQuery());
        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserProfileDto>> UpdateMyProfile([FromBody] UpdateMyProfileCommand command)
    {
        var profile = await _mediator.Send(command);
        return Ok(profile);
    }

    [HttpPost("me/change-password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("me")]
    public async Task<ActionResult> DeleteMyAccount()
    {
        await _mediator.Send(new DeleteMyAccountCommand());
        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PageResult<UserDto>>> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var query = new GetAllUsersQuery
        {
            Paging = new PageRequest { Page = page, PageSize = pageSize },
            Search = search
        };
        var users = await _mediator.Send(query);
        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
        return Ok(user);
    }

    [HttpPatch("{id}/enabled")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateUserEnabled(int id, [FromBody] UpdateUserEnabledDto dto)
    {
        await _mediator.Send(new UpdateUserEnabledCommand { UserId = id, IsEnabled = dto.IsEnabled });
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        await _mediator.Send(new DeleteUserCommand { UserId = id });
        return NoContent();
    }
}

public class UpdateUserEnabledDto
{
    public bool IsEnabled { get; set; }
}
