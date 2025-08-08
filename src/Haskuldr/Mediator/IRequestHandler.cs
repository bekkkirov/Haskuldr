using Haskuldr.Abstractions.Types;
using Haskuldr.Abstractions.Validation;

namespace Haskuldr.Mediator;

public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<Result<TResponse, Error>> HandleAsync(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}

public interface IRequestHandler<in TRequest> where TRequest : IRequest
{
    Task<Option<Error>> HandleAsync(TRequest request, CancellationToken cancellationToken = default);   
}