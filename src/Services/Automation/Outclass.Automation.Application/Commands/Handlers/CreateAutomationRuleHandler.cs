using MediatR;
using Microsoft.Extensions.Logging;
using Outclass.BuildingBlocks.Domain;
using Outclass.Automation.Domain.Entities;

namespace Outclass.Automation.Application.Commands.Handlers;

public class CreateAutomationRuleHandler : IRequestHandler<CreateAutomationRuleCommand, AutomationRuleDto>
{
    private readonly IRepository<AutomationRule> _repository;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<CreateAutomationRuleHandler> _logger;

    public CreateAutomationRuleHandler(IRepository<AutomationRule> repository, ITenantContext tenantContext, ILogger<CreateAutomationRuleHandler> logger)
    {
        _repository = repository;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<AutomationRuleDto> Handle(CreateAutomationRuleCommand request, CancellationToken ct)
    {
        var rule = AutomationRule.Create(
            _tenantContext.TenantId,
            request.Name,
            request.TriggerEvent,
            request.ActionType,
            request.ActionPayload,
            request.EntitySlugFilter);

        await _repository.AddAsync(rule, ct);

        _logger.LogInformation("Automation rule {RuleName} created for event {TriggerEvent}", rule.Name, rule.TriggerEvent);

        return new AutomationRuleDto
        {
            Id = rule.Id,
            Name = rule.Name,
            TriggerEvent = rule.TriggerEvent,
            ActionType = rule.ActionType.ToString(),
            IsActive = rule.IsActive,
            CreatedAt = rule.CreatedAt
        };
    }
}
