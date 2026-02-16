using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Outclass.Metadata.Application.Commands;
using Outclass.Metadata.Application.Queries;

namespace Outclass.Metadata.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EntityDefinitionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EntityDefinitionsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<EntityDefinitionDto>> Create([FromBody] CreateEntityDefinitionCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetBySlug), new { slug = result.Slug }, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<EntityDefinitionDto>>> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetEntityDefinitionsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<EntityDefinitionDto>> GetBySlug(string slug, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetEntityDefinitionBySlugQuery(slug), ct);
        return result == null ? NotFound() : Ok(result);
    }
}
