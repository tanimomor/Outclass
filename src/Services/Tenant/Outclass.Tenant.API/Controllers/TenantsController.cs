using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Outclass.Tenant.Application.Commands;
using Outclass.Tenant.Application.Queries;

namespace Outclass.Tenant.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ProvisionTenantResult>> Provision([FromBody] ProvisionTenantCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.TenantId }, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<TenantDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTenantByIdQuery(id), ct);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("by-slug/{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<TenantDto>> GetBySlug(string slug, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTenantBySlugQuery(slug), ct);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TenantsListDto>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetTenantsQuery(page, pageSize), ct);
        return Ok(result);
    }
}
