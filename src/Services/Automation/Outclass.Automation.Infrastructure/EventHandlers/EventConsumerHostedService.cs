using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Outclass.BuildingBlocks.Infrastructure.EventBus;
using Outclass.BuildingBlocks.Contracts.Document;
using Outclass.BuildingBlocks.Contracts.Workflow;
using System.Text.Json;

namespace Outclass.Automation.Infrastructure.EventHandlers;

public class EventConsumerHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventConsumerHostedService> _logger;

    public EventConsumerHostedService(IServiceProvider serviceProvider, ILogger<EventConsumerHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Automation event consumer starting...");

        try
        {
            var eventBus = _serviceProvider.GetRequiredService<RabbitMqEventBus>();
            var processor = _serviceProvider.GetRequiredService<AutomationEventProcessor>();

            // Bind to document events
            eventBus.BindQueue("automation.document.created", "document.created");
            eventBus.BindQueue("automation.document.updated", "document.updated");
            eventBus.BindQueue("automation.workflow.transitioned", "workflow.transitioned");

            // Subscribe handlers
            eventBus.Subscribe<DocumentCreatedEvent>("automation.document.created", async evt =>
            {
                await processor.ProcessEventAsync(evt.EventType, JsonSerializer.Serialize(evt), evt.TenantId);
            });

            eventBus.Subscribe<DocumentUpdatedEvent>("automation.document.updated", async evt =>
            {
                await processor.ProcessEventAsync(evt.EventType, JsonSerializer.Serialize(evt), evt.TenantId);
            });

            eventBus.Subscribe<WorkflowTransitionedEvent>("automation.workflow.transitioned", async evt =>
            {
                await processor.ProcessEventAsync(evt.EventType, JsonSerializer.Serialize(evt), evt.TenantId);
            });

            _logger.LogInformation("Automation event consumers registered");

            // Keep alive
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Event consumer error");
        }
    }
}
