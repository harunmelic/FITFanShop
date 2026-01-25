using FitFanShop.Application.Modules.Auth.Commands.Login;
using FitFanShop.Application.Modules.Auth.Commands.Logout;
using FitFanShop.Application.Modules.Auth.Commands.Refresh;
using FitFanShop.Application.Modules.Auth.Commands.Register;
using FitFanShop.Application.Modules.Auth.Commands.VerifySecurityAnswer;
using FitFanShop.Application.Modules.Auth.Commands.ResetPassword;
using FitFanShop.Application.Modules.Auth.Queries.GetSecurityQuestion;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginCommandDto>> Register([FromBody] RegisterCommand command, CancellationToken ct)
    {
        return Ok(await mediator.Send(command, ct));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginCommandDto>> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        return Ok(await mediator.Send(command, ct));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginCommandDto>> Refresh([FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        return Ok(await mediator.Send(command, ct));
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task Logout([FromBody] LogoutCommand command, CancellationToken ct)
    {
        await mediator.Send(command, ct);
    }

    [AllowAnonymous]
    [HttpGet("security-question")]
    public async Task<ActionResult<GetSecurityQuestionDto>> GetSecurityQuestion([FromQuery] string email, CancellationToken ct)
    {
        return Ok(await mediator.Send(new GetSecurityQuestionQuery(email), ct));
    }

    [AllowAnonymous]
    [HttpPost("verify-security-answer")]
    public async Task<ActionResult<VerifySecurityAnswerDto>> VerifySecurityAnswer([FromBody] VerifySecurityAnswerCommand command, CancellationToken ct)
    {
        return Ok(await mediator.Send(command, ct));
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<ActionResult<ResetPasswordDto>> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken ct)
    {
        return Ok(await mediator.Send(command, ct));
    }
}

