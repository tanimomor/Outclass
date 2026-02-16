using MediatR;
using FluentValidation;

namespace Outclass.Workflow.Application.Commands;

public record CreateWorkflowDefinitionCommand : IRequest<WorkflowDefinitionDto>
{
    public string Name { get; init; } = default!;
    public string EntitySlug { get; init; } = default!;
    public string InitialState { get; init; } = default!;
    public List<TransitionRuleDto> Transitions { get; init; } = new();
}

public record TransitionRuleDto
{
    public string FromState { get; init; } = default!;
    public string ToState { get; init; } = default!;
    public string? RequiredRole { get; init; }
}

public record TransitionDocumentCommand : IRequest<WorkflowInstanceDto>
{
    public Guid DocumentId { get; init; }
    public string ToState { get; init; } = default!;
}

public record WorkflowDefinitionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string EntitySlug { get; init; } = default!;
    public string InitialState { get; init; } = default!;
    public List<TransitionRuleDto> Transitions { get; init; } = new();
}

public record WorkflowInstanceDto
{
    public Guid Id { get; init; }
    public Guid DocumentId { get; init; }
    public string CurrentState { get; init; } = default!;
    public List<string> AvailableTransitions { get; init; } = new();
}

public class CreateWorkflowDefinitionValidator : AbstractValidator<CreateWorkflowDefinitionCommand>
{
    public CreateWorkflowDefinitionValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.EntitySlug).NotEmpty().MaximumLength(100);
        RuleFor(x => x.InitialState).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Transitions).NotEmpty();
    }
}
