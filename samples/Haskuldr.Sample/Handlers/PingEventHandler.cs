using Haskuldr.Abstractions.EventBus;
using Haskuldr.Bus;

namespace Haskuldr.Sample.Handlers;

public record PingEvent(string Message) : IEvent;

public class PingEventHandler : IEventHandler<PingEvent>
{
    public async Task HandleAsync(PingEvent @event, CancellationToken cancellationToken = default)
    {
        await Task.Delay(100, cancellationToken);
    }
}