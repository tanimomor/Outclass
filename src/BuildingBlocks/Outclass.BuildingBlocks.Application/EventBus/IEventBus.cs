using Outclass.BuildingBlocks.Contracts;

namespace Outclass.BuildingBlocks.Application.EventBus;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : IIntegrationEvent;
}

public interface IEventHandler<in T> where T : IIntegrationEvent
{
    Task HandleAsync(T @event, CancellationToken ct = default);
}
