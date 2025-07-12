using Microsoft.AspNetCore.Routing;

namespace Haskuldr.WebApi.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}