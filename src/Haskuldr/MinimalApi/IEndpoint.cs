using Microsoft.AspNetCore.Routing;

namespace Haskuldr.MinimalApi;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}