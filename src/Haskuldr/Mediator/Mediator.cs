using Haskuldr.Abstractions.Types;
using Haskuldr.Abstractions.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Haskuldr.Mediator;

public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public async Task<Result<TResponse, Error>> SendAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
        where TResponse : notnull
    {
        var queryHandlers = serviceProvider
                            .GetServices<IRequestHandler<TRequest, TResponse>>()
                            .ToArray();
        
        var result = await queryHandlers
            .Single()
            .HandleAsync(request, cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    public async Task<Option<Error>> SendAsync<TRequest>(
        TRequest request,
        CancellationToken cancellationToken = default) 
        where TRequest : IRequest
    {
        var commandHandlers = serviceProvider
                              .GetServices<IRequestHandler<TRequest>>()
                              .ToArray();
        
        var result = await commandHandlers
                           .Single()
                           .HandleAsync(request, cancellationToken)
                           .ConfigureAwait(false);

        return result;
    }
}