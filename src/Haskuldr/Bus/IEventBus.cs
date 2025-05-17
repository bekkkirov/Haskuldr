using Haskuldr.Abstractions.EventBus;

namespace Haskuldr.Bus;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
}