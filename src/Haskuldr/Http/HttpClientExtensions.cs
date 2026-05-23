using System.Diagnostics.CodeAnalysis;

namespace Haskuldr.Http;

/// <summary>
/// Provides fluent extensions for <see cref="HttpClient"/>.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Starts building a fluent HTTP request.
    /// </summary>
    /// <example>
    /// <code>
    /// var result = await client.Request(HttpMethod.Get, "users/{id}")
    ///     .RouteParam("id", userId)
    ///     .SendAsync&lt;UserDto&gt;(cancellationToken);
    /// </code>
    /// </example>
    public static HttpRequestBuilder Request(
        this HttpClient httpClient,
        HttpMethod method,
        string? route = null)
    {
        return new HttpRequestBuilder(httpClient, method, route);
    }

    /// <summary>
    /// Starts building a fluent <c>GET</c> request.
    /// </summary>
    public static HttpRequestBuilder Get(
        this HttpClient httpClient,
        [StringSyntax("Route")] string? route = null) => httpClient.Request(HttpMethod.Get, route);

    /// <summary>
    /// Starts building a fluent <c>POST</c> request.
    /// </summary>
    public static HttpRequestBuilder Post(
        this HttpClient httpClient,
        [StringSyntax("Route")] string? route = null) =>
        httpClient.Request(HttpMethod.Post, route);

    /// <summary>
    /// Starts building a fluent <c>PUT</c> request.
    /// </summary>
    public static HttpRequestBuilder Put(
        this HttpClient httpClient,
        [StringSyntax("Route")] string? route = null) => httpClient.Request(HttpMethod.Put, route);

    /// <summary>
    /// Starts building a fluent <c>PATCH</c> request.
    /// </summary>
    public static HttpRequestBuilder Patch(
        this HttpClient httpClient,
        [StringSyntax("Route")] string? route = null) => httpClient.Request(HttpMethod.Patch, route);

    /// <summary>
    /// Starts building a fluent <c>DELETE</c> request.
    /// </summary>
    public static HttpRequestBuilder Delete(
        this HttpClient httpClient,
        [StringSyntax("Route")] string? route = null) => httpClient.Request(HttpMethod.Delete, route);
}