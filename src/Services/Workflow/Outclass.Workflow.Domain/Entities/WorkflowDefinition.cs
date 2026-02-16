using Outclass.BuildingBlocks.Domain;

namespace Outclass.Workflow.Domain.Entities;

public class WorkflowDefinition : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string EntitySlug { get; private set; } = default!;
    public string InitialState { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    private readonly List<WorkflowTransitionRule> _transitions = new();
    public IReadOnlyCollection<WorkflowTransitionRule> Transitions => _transitions.AsReadOnly();

    private WorkflowDefinition() { }

    public static WorkflowDefinition Create(Guid tenantId, string name, string entitySlug, string initialState)
    {
        var wf = new WorkflowDefinition
        {
            Name = name,
            EntitySlug = entitySlug.ToLowerInvariant(),
            InitialState = initialState
        };
        wf.SetTenant(tenantId);
        return wf;
    }

    public WorkflowTransitionRule AddTransition(string fromState, string toState, string? requiredRole = null, string? condition = null)
    {
        var rule = new WorkflowTransitionRule
        {
            WorkflowDefinitionId = Id,
            FromState = fromState,
            ToState = toState,
            RequiredRole = requiredRole,
            Condition = condition
        };
        rule.SetTenant(TenantId);
        _transitions.Add(rule);
        return rule;
    }

    public bool CanTransition(string fromState, string toState, IReadOnlyList<string> userRoles)
    {
        var rule = _transitions.FirstOrDefault(t =>
            t.FromState == fromState && t.ToState == toState);

        if (rule == null) return false;
        if (string.IsNullOrEmpty(rule.RequiredRole)) return true;
        return userRoles.Contains(rule.RequiredRole);
    }

    public IReadOnlyList<string> GetAvailableTransitions(string currentState, IReadOnlyList<string> userRoles)
    {
        return _transitions
            .Where(t => t.FromState == currentState)
            .Where(t => string.IsNullOrEmpty(t.RequiredRole) || userRoles.Contains(t.RequiredRole))
            .Select(t => t.ToState)
            .ToList();
    }
}

public class WorkflowTransitionRule : BaseEntity
{
    public Guid WorkflowDefinitionId { get; set; }
    public string FromState { get; set; } = default!;
    public string ToState { get; set; } = default!;
    public string? RequiredRole { get; set; }
    public string? Condition { get; set; } // JSON-based condition
}

public class WorkflowInstance : BaseEntity
{
    public Guid WorkflowDefinitionId { get; private set; }
    public Guid DocumentId { get; private set; }
    public string CurrentState { get; private set; } = default!;

    private readonly List<WorkflowTransitionLog> _transitionLogs = new();
    public IReadOnlyCollection<WorkflowTransitionLog> TransitionLogs => _transitionLogs.AsReadOnly();

    private WorkflowInstance() { }

    public static WorkflowInstance Create(Guid tenantId, Guid workflowDefinitionId, Guid documentId, string initialState)
    {
        var instance = new WorkflowInstance
        {
            WorkflowDefinitionId = workflowDefinitionId,
            DocumentId = documentId,
            CurrentState = initialState
        };
        instance.SetTenant(tenantId);
        return instance;
    }

    public void Transition(string toState, string? userId)
    {
        var log = new WorkflowTransitionLog
        {
            WorkflowInstanceId = Id,
            FromState = CurrentState,
            ToState = toState,
            TransitionedBy = userId,
            TransitionedAt = DateTime.UtcNow
        };
        log.SetTenant(TenantId);
        _transitionLogs.Add(log);

        var previousState = CurrentState;
        CurrentState = toState;

        AddDomainEvent(new WorkflowTransitionedDomainEvent(Id, DocumentId, previousState, toState, userId, TenantId));
    }
}

public class WorkflowTransitionLog : BaseEntity
{
    public Guid WorkflowInstanceId { get; set; }
    public string FromState { get; set; } = default!;
    public string ToState { get; set; } = default!;
    public string? TransitionedBy { get; set; }
    public DateTime TransitionedAt { get; set; }
}

public record WorkflowTransitionedDomainEvent : DomainEvent
{
    public override string EventType => "workflow.transitioned";
    public Guid InstanceId { get; }
    public Guid DocumentId { get; }
    public string FromState { get; }
    public string ToState { get; }
    public string? TransitionedBy { get; }
    public Guid TenantIdValue { get; }

    public WorkflowTransitionedDomainEvent(Guid instanceId, Guid documentId, string fromState, string toState, string? transitionedBy, Guid tenantId)
    {
        InstanceId = instanceId;
        DocumentId = documentId;
        FromState = fromState;
        ToState = toState;
        TransitionedBy = transitionedBy;
        TenantIdValue = tenantId;
    }
}
