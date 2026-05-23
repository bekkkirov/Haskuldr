using Haskuldr.Shared;
using Microsoft.AspNetCore.Routing.Patterns;

namespace Haskuldr.Http.Internal;

internal static class HttpRequestMaterializer
{
    internal static HttpRequestMessage Materialize(HttpRequestBuilder builder)
    {
        if (builder.HttpClient.BaseAddress is null)
        {
            throw new InvalidOperationException(ErrorMessages.HttpInvalidBaseAddress);
        }

        if (builder.Timeout.HasValue)
        {
            builder.HttpClient.Timeout = builder.Timeout.Value;
        }
        
        var route = GetRoute(builder);
        var queryParams = GetQueryParams(builder);

        var requestUri = new Uri(route + queryParams, UriKind.Relative);
        
        var message = new HttpRequestMessage(builder.Method, requestUri);
        
        SetContent(message, builder.Body);
        SetHeaders(message, builder.Headers);

        return message;
    }

    private static string? GetRoute(HttpRequestBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder.HttpClient.BaseAddress);
        
        if (string.IsNullOrEmpty(builder.Route))
        {
            return builder.RouteParams.Count > 0 
                ? throw new InvalidOperationException(ErrorMessages.HttpRouteParamsMismatch) 
                : null;
        }
        
        var routePattern = RoutePatternFactory.Parse(builder.Route);

        if (routePattern.Parameters.Count != builder.RouteParams.Count)
        {
            throw new InvalidOperationException(ErrorMessages.HttpRouteParamsMismatch);
        }

        var route = builder.Route;

        foreach (var param in routePattern.Parameters)
        {
            if (!builder.RouteParams.TryGetValue(param.Name, out var value))
            {
                throw new InvalidOperationException(ErrorMessages.HttpRouteParamsMismatch);
            }
            
            route = route.Replace($"{{{param}}}", Uri.EscapeDataString(value), StringComparison.OrdinalIgnoreCase);
        }

        return Uri.TryCreate(route, UriKind.Relative, out _)
            ? route
            : throw new InvalidOperationException(ErrorMessages.InvalidRoute);
    }

    private static string? GetQueryParams(HttpRequestBuilder builder)
    {
        var queryString = string.Join(
            "&",
            builder.QueryParams.Select(pair =>
            {
                var key = Uri.EscapeDataString(pair.Key);

                var value = pair.Value.Map(
                    Uri.EscapeDataString,
                    x => string.Join($"&{key}=", x.Select(Uri.EscapeDataString)));
                
                return $"{key}={value}";
            }));

        if (string.IsNullOrWhiteSpace(queryString))
        {
            return null;
        }
        
        string separator;

        if (builder.Route is null)
        {
            separator = string.Empty;
        }
        else if (builder.Route.Contains('?', StringComparison.OrdinalIgnoreCase))
        {
            separator = builder.Route[^1] is '&' or '?' 
                ? string.Empty 
                : "&";
        }
        else
        {
            separator = "?";
        }

        var finalQuery = $"{separator}{queryString}";
        
        return Uri.TryCreate(finalQuery, UriKind.Relative, out _)
            ? finalQuery
            : throw new InvalidOperationException(ErrorMessages.InvalidQueryString);
    }

    private static void SetContent(
        HttpRequestMessage requestMessage,
        HttpContent? body)
    {
        requestMessage.Content = body;
    }
    
    private static void SetHeaders(
        HttpRequestMessage requestMessage,
        IReadOnlyDictionary<string, string> headers)
    {
        foreach (var header in headers)
        {
            if (requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value))
            {
                continue;
            }

            if (requestMessage.Content is not null &&
                requestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value))
            {
                continue;
            }

            throw new InvalidOperationException($"{ErrorMessages.HttpInvalidHeader}: {header.Key}");
        }
    }
}
