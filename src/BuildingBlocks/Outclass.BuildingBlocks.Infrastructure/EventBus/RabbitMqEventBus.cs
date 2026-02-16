using System.Text;
using System.Text.Json;
using Outclass.BuildingBlocks.Application.EventBus;
using Outclass.BuildingBlocks.Contracts;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Outclass.BuildingBlocks.Infrastructure.EventBus;

public sealed class RabbitMqEventBus : IEventBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqEventBus> _logger;
    private const string ExchangeName = "outclass.events";

    public RabbitMqEventBus(IConnection connection, ILogger<RabbitMqEventBus> logger)
    {
        _connection = connection;
        _channel = connection.CreateModel();
        _logger = logger;
        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
    }

    public Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : IIntegrationEvent
    {
        var routingKey = @event.EventType;
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event, @event.GetType()));

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.MessageId = @event.EventId.ToString();
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        properties.Headers = new Dictionary<string, object>
        {
            ["tenant_id"] = @event.TenantId.ToString(),
            ["event_type"] = @event.EventType
        };

        _channel.BasicPublish(ExchangeName, routingKey, properties, body);
        _logger.LogInformation("Published event {EventType} with id {EventId}", @event.EventType, @event.EventId);

        return Task.CompletedTask;
    }

    public void Subscribe<T>(string queueName, Func<T, Task> handler) where T : IIntegrationEvent
    {
        _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            try
            {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                var @event = JsonSerializer.Deserialize<T>(body);
                if (@event != null)
                {
                    await handler(@event);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event from queue {Queue}", queueName);
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queueName, false, consumer);
    }

    public void BindQueue(string queueName, string routingKey)
    {
        _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(queueName, ExchangeName, routingKey);
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}
