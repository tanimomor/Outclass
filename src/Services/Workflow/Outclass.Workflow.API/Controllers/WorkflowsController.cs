using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Outclass.Workflow.Application.Commands;

namespace Outclass.Workflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkflowsController : ControllerBase
{
    private readonly IMediator _mediator;
    public WorkflowsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("definitions")]
    public async Task<ActionResult<WorkflowDefinitionDto>> CreateDefinition([FromBody] CreateWorkflowDefinitionCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Created($"/api/workflows/definitions/{result.Id}", result);
    }

    [HttpPost("transition")]
    public async Task<ActionResult<WorkflowInstanceDto>> Transition([FromBody] TransitionDocumentCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
}
