using Haskuldr.Abstractions.Types;
using Haskuldr.Abstractions.Validation;

namespace Haskuldr.Mediator;

public interface IMediator
{
    Task<Option<Error>> SendAsync<TRequest>(
        TRequest request,
        CancellationToken cancellationToken = default) where TRequest : IRequest;
    
    Task<Result<TResponse, Error>> SendAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>;
}