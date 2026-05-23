using System.Net.Http.Json;
using Haskuldr.Abstractions.Types;
using Haskuldr.Http.Internal;
using Haskuldr.Shared;
using Microsoft.Net.Http.Headers;
using QueryParam = Haskuldr.Abstractions.Types.Union<string, string[]>;

namespace Haskuldr.Http;

/// <summary>
/// Builds and sends a single HTTP request through <see cref="HttpClient"/>.
/// </summary>
/// <remarks>
/// Each builder instance is single-use.
/// </remarks>
public sealed class HttpRequestBuilder
{
    private bool _isSent;
    
    private readonly Dictionary<string, string> _routeParams = [];
    private readonly Dictionary<string, QueryParam> _queryParams = [];
    private readonly Dictionary<string, string> _headers = [];
    
    public HttpRequestBuilder(
        HttpClient httpClient,
        HttpMethod method,
        string? route)
    {
        HttpClient = httpClient;
        Method = method;
        Route = route;
    }
    
    public HttpClient HttpClient { get; private set; }
    
    public HttpMethod Method { get; private set; }
    
    public string? Route { get; private set; }

    public HttpContent? Body { get; private set; }

    public IReadOnlyDictionary<string, string> RouteParams => _routeParams.AsReadOnly();
    
    public IReadOnlyDictionary<string, QueryParam> QueryParams => _queryParams.AsReadOnly();
    
    public IReadOnlyDictionary<string, string> Headers => _headers.AsReadOnly();

    public TimeSpan? Timeout { get; private set; }
    
    /// <summary>
    /// Adds or replaces route parameter.
    /// </summary>
    public HttpRequestBuilder WithRouteParam(
        string name,
        string value)
    {
        EnsureMutable();

        _routeParams[name] = value;

        return this;
    }

    /// <summary>
    /// Adds or replaces route parameters.
    /// </summary>
    public HttpRequestBuilder WithRouteParams(Dictionary<string, string> values)
    {
        EnsureMutable();

        foreach (var (name, value) in values)
        {
            _routeParams[name] = value;
        }

        return this;
    }

    /// <summary>
    /// Adds or replaces query parameter.
    /// </summary>
    public HttpRequestBuilder WithQueryParam(
        string name,
        QueryParam value)
    {
        EnsureMutable();

        _queryParams[name] = value;

        return this;
    }

    /// <summary>
    /// Adds or replaces query parameters.
    /// </summary>
    public HttpRequestBuilder WithQueryParams(IReadOnlyDictionary<string, QueryParam> values)
    {
        EnsureMutable();

        foreach (var (name, value) in values)
        {
            _queryParams[name] = value;
        }

        return this;
    }

    /// <summary>
    /// Adds or replaces the request header.
    /// </summary>
    public HttpRequestBuilder WithHeader(
        string name,
        string value)
    {
        EnsureMutable();

        _headers[name] = value;

        return this;
    }

    /// <summary>
    /// Adds or replaces request headers.
    /// </summary>
    public HttpRequestBuilder WithHeaders(IReadOnlyDictionary<string, string> values)
    {
        EnsureMutable();

        foreach (var (name, value) in values)
        {
            _headers[name] = value;
        }

        return this;
    }

    public HttpRequestBuilder WithBearer(string bearer)
    {
        EnsureMutable();
        
        _headers[HeaderNames.Authorization] = $"Bearer {bearer}";

        return this;
    }
    
    /// <summary>
    /// Sets the request body to an existing <see cref="HttpContent"/> instance.
    /// </summary>
    public HttpRequestBuilder WithBody(HttpContent content)
    {
        EnsureMutable();

        Body = content;

        return this;
    }
    
    /// <summary>
    /// Sets the request body to JSON content using the default <see cref="System.Text.Json.JsonSerializer"/> behavior.
    /// </summary>
    public HttpRequestBuilder WithJsonBody<T>(T value)
    {
        EnsureMutable();

        Body = JsonContent.Create(value);

        return this;
    }

    /// <summary>
    /// Sets the request timeout.
    /// </summary>
    public HttpRequestBuilder WithTimeout(TimeSpan timeout)
    {
        EnsureMutable();

        Timeout = timeout;

        return this;
    }

    /// <summary>
    /// Sends the request and deserializes successful and non-successful bodies into separate types.
    /// </summary>
    public async Task<Result<TSuccess, TError>> SendAsync<TSuccess, TError>(CancellationToken cancellationToken = default)
        where TSuccess : notnull
        where TError : notnull
    {
        using var materializedRequest = PrepareRequest();

        return await HttpRequestHandler
            .SendAsync<TSuccess, TError>(
                HttpClient,
                materializedRequest,
                cancellationToken)
            .ConfigureAwait(false);
    }
    
    public async Task<Result<TSuccess, HttpError>> SendAsync<TSuccess>(CancellationToken cancellationToken = default)
        where TSuccess : notnull
    {
        using var materializedRequest = PrepareRequest();

        return await HttpRequestHandler
                     .SendAsync<TSuccess, HttpError>(
                         HttpClient,
                         materializedRequest,
                         cancellationToken)
                     .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Sends the request and deserializes successful and non-successful bodies into separate types.
    /// </summary>
    public async Task<Result<Option<TSuccess>, TError>> SendOptionalAsync<TSuccess, TError>(CancellationToken cancellationToken = default)
        where TSuccess : notnull
        where TError : notnull
    {
        using var materializedRequest = PrepareRequest();

        return await HttpRequestHandler
                     .SendOptionalAsync<TSuccess, TError>(
                         HttpClient,
                         materializedRequest,
                         cancellationToken)
                     .ConfigureAwait(false);
    }

    /// <summary>
    /// Sends the request when no success body is expected and a custom non-success body should be deserialized.
    /// </summary>
    public async Task<Option<TError>> SendAsync<TError>(CancellationToken cancellationToken = default)
        where TError : notnull
    {
        using var materializedRequest = PrepareRequest();

        return await HttpRequestHandler
            .SendAsync<TError>(
                HttpClient,
                materializedRequest,
                cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sends the request when no success body is expected and returns non-success responses as <see cref="HttpError"/>.
    /// </summary>
    public async Task<Option<HttpError>> SendAsync(CancellationToken cancellationToken = default)
    {
        using var materializedRequest = PrepareRequest();

        return await HttpRequestHandler
            .SendAsync(HttpClient, materializedRequest, cancellationToken)
            .ConfigureAwait(false);
    }

    private void EnsureMutable()
    {
        if (_isSent)
        {
            throw new InvalidOperationException(ErrorMessages.HttpRequestAlreadySent);
        }
    }

    private HttpRequestMessage PrepareRequest()
    {
        EnsureMutable();
        
        _isSent = true;

        return HttpRequestMaterializer.Materialize(this);
    }
}
