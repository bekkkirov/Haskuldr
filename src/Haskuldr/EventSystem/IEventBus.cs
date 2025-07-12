using Haskuldr.Abstractions.EventSystem;

namespace Haskuldr.EventSystem;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
}