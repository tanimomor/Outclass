using MediatR;
using Microsoft.Extensions.Logging;
using Outclass.BuildingBlocks.Application.EventBus;
using Outclass.BuildingBlocks.Contracts.Workflow;
using Outclass.BuildingBlocks.Domain;
using Outclass.Workflow.Domain.Entities;

namespace Outclass.Workflow.Application.Commands.Handlers;

public class CreateWorkflowDefinitionHandler : IRequestHandler<CreateWorkflowDefinitionCommand, WorkflowDefinitionDto>
{
    private readonly IRepository<WorkflowDefinition> _repository;
    private readonly ITenantContext _tenantContext;

    public CreateWorkflowDefinitionHandler(IRepository<WorkflowDefinition> repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    public async Task<WorkflowDefinitionDto> Handle(CreateWorkflowDefinitionCommand request, CancellationToken ct)
    {
        var wf = WorkflowDefinition.Create(_tenantContext.TenantId, request.Name, request.EntitySlug, request.InitialState);

        foreach (var t in request.Transitions)
        {
            wf.AddTransition(t.FromState, t.ToState, t.RequiredRole);
        }

        await _repository.AddAsync(wf, ct);

        return new WorkflowDefinitionDto
        {
            Id = wf.Id,
            Name = wf.Name,
            EntitySlug = wf.EntitySlug,
            InitialState = wf.InitialState,
            Transitions = wf.Transitions.Select(t => new TransitionRuleDto
            {
                FromState = t.FromState,
                ToState = t.ToState,
                RequiredRole = t.RequiredRole
            }).ToList()
        };
    }
}

public class TransitionDocumentHandler : IRequestHandler<TransitionDocumentCommand, WorkflowInstanceDto>
{
    private readonly IRepository<WorkflowInstance> _instanceRepo;
    private readonly IRepository<WorkflowDefinition> _definitionRepo;
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUser _currentUser;
    private readonly IEventBus _eventBus;
    private readonly ILogger<TransitionDocumentHandler> _logger;

    public TransitionDocumentHandler(
        IRepository<WorkflowInstance> instanceRepo,
        IRepository<WorkflowDefinition> definitionRepo,
        ITenantContext tenantContext,
        ICurrentUser currentUser,
        IEventBus eventBus,
        ILogger<TransitionDocumentHandler> logger)
    {
        _instanceRepo = instanceRepo;
        _definitionRepo = definitionRepo;
        _tenantContext = tenantContext;
        _currentUser = currentUser;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<WorkflowInstanceDto> Handle(TransitionDocumentCommand request, CancellationToken ct)
    {
        var instances = await _instanceRepo.FindAsync(i => i.DocumentId == request.DocumentId, ct);
        var instance = instances.FirstOrDefault()
                       ?? throw new NotFoundException("WorkflowInstance", request.DocumentId);

        var definition = await _definitionRepo.GetByIdAsync(instance.WorkflowDefinitionId, ct)
                         ?? throw new NotFoundException("WorkflowDefinition", instance.WorkflowDefinitionId);

        if (!definition.CanTransition(instance.CurrentState, request.ToState, _currentUser.Roles))
            throw new ForbiddenException($"Transition from '{instance.CurrentState}' to '{request.ToState}' is not allowed.");

        var fromState = instance.CurrentState;
        instance.Transition(request.ToState, _currentUser.UserId);
        await _instanceRepo.UpdateAsync(instance, ct);

        await _eventBus.PublishAsync(new WorkflowTransitionedEvent
        {
            TenantId = _tenantContext.TenantId,
            DocumentId = request.DocumentId,
            FromState = fromState,
            ToState = request.ToState,
            TransitionedBy = _currentUser.UserId
        }, ct);

        _logger.LogInformation("Document {DocumentId} transitioned from {From} to {To}",
            request.DocumentId, fromState, request.ToState);

        return new WorkflowInstanceDto
        {
            Id = instance.Id,
            DocumentId = instance.DocumentId,
            CurrentState = instance.CurrentState,
            AvailableTransitions = definition.GetAvailableTransitions(instance.CurrentState, _currentUser.Roles).ToList()
        };
    }
}
