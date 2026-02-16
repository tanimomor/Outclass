using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Outclass.BuildingBlocks.Domain;
using Outclass.BuildingBlocks.Infrastructure.EventBus;
using Outclass.Automation.Domain.Entities;

namespace Outclass.Automation.Infrastructure.EventHandlers;

public class AutomationEventProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AutomationEventProcessor> _logger;

    public AutomationEventProcessor(IServiceProvider serviceProvider, ILogger<AutomationEventProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task ProcessEventAsync(string eventType, string payload, Guid tenantId)
    {
        using var scope = _serviceProvider.CreateScope();
        var ruleRepo = scope.ServiceProvider.GetRequiredService<IRepository<AutomationRule>>();
        var logRepo = scope.ServiceProvider.GetRequiredService<IRepository<AutomationExecutionLog>>();

        var rules = await ruleRepo.FindAsync(r => r.TriggerEvent == eventType && r.IsActive);

        foreach (var rule in rules)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                _logger.LogInformation("Executing automation rule {RuleName} for event {EventType}", rule.Name, eventType);

                // Execute action based on type
                switch (rule.ActionType)
                {
                    case AutomationActionType.Webhook:
                        await ExecuteWebhookAsync(rule.ActionPayload);
                        break;
                    case AutomationActionType.SendEmail:
                        _logger.LogInformation("Email action triggered for rule {RuleName}", rule.Name);
                        break;
                    default:
                        _logger.LogInformation("Action {ActionType} executed for rule {RuleName}", rule.ActionType, rule.Name);
                        break;
                }

                sw.Stop();
                var successLog = AutomationExecutionLog.CreateSuccess(tenantId, rule.Id, eventType, sw.ElapsedMilliseconds);
                await logRepo.AddAsync(successLog);
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Automation rule {RuleName} failed", rule.Name);
                var failureLog = AutomationExecutionLog.CreateFailure(tenantId, rule.Id, eventType, ex.Message, sw.ElapsedMilliseconds);
                await logRepo.AddAsync(failureLog);
            }
        }
    }

    private async Task ExecuteWebhookAsync(string payload)
    {
        // Webhook execution placeholder
        await Task.CompletedTask;
        _logger.LogInformation("Webhook executed with payload");
    }
}
