﻿using Haskuldr.Abstractions.EventBus;

namespace Haskuldr.Bus;

public interface IEventHandler<in TEvent> where TEvent : IEvent 
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}