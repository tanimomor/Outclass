using MediatR;
using FluentValidation;
using Outclass.Automation.Domain.Entities;

namespace Outclass.Automation.Application.Commands;

public record CreateAutomationRuleCommand : IRequest<AutomationRuleDto>
{
    public string Name { get; init; } = default!;
    public string TriggerEvent { get; init; } = default!;
    public string? EntitySlugFilter { get; init; }
    public AutomationActionType ActionType { get; init; }
    public string ActionPayload { get; init; } = default!;
}

public record AutomationRuleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string TriggerEvent { get; init; } = default!;
    public string ActionType { get; init; } = default!;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}

public class CreateAutomationRuleValidator : AbstractValidator<CreateAutomationRuleCommand>
{
    public CreateAutomationRuleValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TriggerEvent).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ActionPayload).NotEmpty();
    }
}
