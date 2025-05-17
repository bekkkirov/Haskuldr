using Haskuldr.Bus;
using Haskuldr.MinimalApi;
using Haskuldr.Sample.Handlers;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Haskuldr.Sample.Endpoints.Test.V1;

public sealed class GetString : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/test",
            async Task<Results<Ok<string>, BadRequest<string>>> (
                IEventBus eventBus,
                CancellationToken cancellationToken) =>
            {
                await eventBus.PublishAsync(new PingEvent("test"), cancellationToken);
                
                return TypedResults.Ok("test");
            }); 
    }
}