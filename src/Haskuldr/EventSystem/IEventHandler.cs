using Haskuldr.Abstractions.EventSystem;

namespace Haskuldr.EventSystem;

public interface IEventHandler<in TEvent> where TEvent : IEvent 
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}