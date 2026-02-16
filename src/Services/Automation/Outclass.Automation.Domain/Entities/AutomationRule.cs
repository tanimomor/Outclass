using Outclass.BuildingBlocks.Domain;

namespace Outclass.Automation.Domain.Entities;

public class AutomationRule : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string TriggerEvent { get; private set; } = default!; // e.g. "document.created"
    public string? EntitySlugFilter { get; private set; }
    public AutomationActionType ActionType { get; private set; }
    public string ActionPayload { get; private set; } = default!; // JSON
    public bool IsActive { get; private set; } = true;

    private AutomationRule() { }

    public static AutomationRule Create(Guid tenantId, string name, string triggerEvent,
        AutomationActionType actionType, string actionPayload, string? entitySlugFilter = null)
    {
        var rule = new AutomationRule
        {
            Name = name,
            TriggerEvent = triggerEvent,
            EntitySlugFilter = entitySlugFilter,
            ActionType = actionType,
            ActionPayload = actionPayload
        };
        rule.SetTenant(tenantId);
        return rule;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

public class AutomationExecutionLog : BaseEntity
{
    public Guid AutomationRuleId { get; private set; }
    public string TriggerEvent { get; private set; } = default!;
    public string Status { get; private set; } = default!; // "Success", "Failed"
    public string? ErrorMessage { get; private set; }
    public DateTime ExecutedAt { get; private set; } = DateTime.UtcNow;
    public long DurationMs { get; private set; }

    private AutomationExecutionLog() { }

    public static AutomationExecutionLog CreateSuccess(Guid tenantId, Guid ruleId, string triggerEvent, long durationMs)
    {
        var log = new AutomationExecutionLog
        {
            AutomationRuleId = ruleId,
            TriggerEvent = triggerEvent,
            Status = "Success",
            DurationMs = durationMs
        };
        log.SetTenant(tenantId);
        return log;
    }

    public static AutomationExecutionLog CreateFailure(Guid tenantId, Guid ruleId, string triggerEvent, string error, long durationMs)
    {
        var log = new AutomationExecutionLog
        {
            AutomationRuleId = ruleId,
            TriggerEvent = triggerEvent,
            Status = "Failed",
            ErrorMessage = error,
            DurationMs = durationMs
        };
        log.SetTenant(tenantId);
        return log;
    }
}

public enum AutomationActionType
{
    Webhook,
    SendEmail,
    UpdateDocument,
    TransitionWorkflow,
    ExecuteScript
}
