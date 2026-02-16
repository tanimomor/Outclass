using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Outclass.Automation.Application.Commands;

namespace Outclass.Automation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AutomationController : ControllerBase
{
    private readonly IMediator _mediator;
    public AutomationController(IMediator mediator) => _mediator = mediator;

    [HttpPost("rules")]
    public async Task<ActionResult<AutomationRuleDto>> CreateRule([FromBody] CreateAutomationRuleCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Created($"/api/automation/rules/{result.Id}", result);
    }
}
