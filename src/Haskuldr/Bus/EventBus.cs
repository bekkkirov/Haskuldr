using System.Reflection;
using Haskuldr.Abstractions.EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace Haskuldr.Bus;

public class EventBus(IServiceProvider serviceProvider) : IEventBus
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        var handlers = serviceProvider
                       .GetServices<IEventHandler<TEvent>>()
                       .OrderBy(x =>
                       {
                           var orderAttribute = x.GetType()
                                                 .GetCustomAttribute<OrderAttribute>();

                           return orderAttribute?.Order ?? int.MaxValue;
                       })
                       .ToList();

        if (handlers.Count == 0)
        {
            throw new InvalidOperationException($"No handlers found for type {typeof(TEvent).Name}");
        }
        
        foreach (var handler in handlers)
        {
            await handler.HandleAsync(@event, cancellationToken).ConfigureAwait(false);
        }
    }
}