using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Outclass.Identity.Application.Commands;
using Outclass.Identity.Application.Queries;

namespace Outclass.Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<RegisterUserResult>> Register([FromBody] RegisterUserCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetUser), new { id = result.UserId }, result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResult>> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResult>> Refresh([FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpGet("users/{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetUser(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id), ct);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UsersListDto>> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetUsersQuery(page, pageSize), ct);
        return Ok(result);
    }
}
