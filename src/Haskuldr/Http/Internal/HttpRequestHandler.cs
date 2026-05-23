using System.Net.Http.Json;
using System.Text.Json;
using Haskuldr.Abstractions.Types;
using Haskuldr.Shared;

namespace Haskuldr.Http.Internal;

internal static class HttpRequestHandler
{
    internal static async Task<Result<TSuccess, TError>> SendAsync<TSuccess, TError>(
        HttpClient httpClient,
        HttpRequestMessage request,
        CancellationToken cancellationToken)
        where TSuccess : notnull
        where TError : notnull
    {
        using var response = await httpClient
            .SendAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
        {
            return await DeserializeRequiredAsync<TSuccess>(response.Content, cancellationToken).ConfigureAwait(false);
        }
        
        return await DeserializeRequiredAsync<TError>(response.Content, cancellationToken).ConfigureAwait(false);
    }
    
    internal static async Task<Result<Option<TSuccess>, TError>> SendOptionalAsync<TSuccess, TError>(
        HttpClient httpClient,
        HttpRequestMessage request,
        CancellationToken cancellationToken)
        where TSuccess : notnull
        where TError : notnull
    {
        using var response = await httpClient
                                   .SendAsync(request, cancellationToken)
                                   .ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
        {
            return await DeserializeOptionalAsync<TSuccess>(response.Content, cancellationToken).ConfigureAwait(false);
        }
        
        return await DeserializeRequiredAsync<TError>(response.Content, cancellationToken).ConfigureAwait(false);
    }
    
    internal static async Task<Option<TError>> SendAsync<TError>(
        HttpClient httpClient,
        HttpRequestMessage request,
        CancellationToken cancellationToken)
        where TError : notnull
    {
        using var response = await httpClient
            .SendAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
        {
            return default;
        }

        return await DeserializeRequiredAsync<TError>(response.Content, cancellationToken).ConfigureAwait(false);
    }

    internal static async Task<Option<HttpError>> SendAsync(
        HttpClient httpClient,
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        using var response = await httpClient
            .SendAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
        {
            return Option<HttpError>.None;
        }

        return await DeserializeHttpErrorAsync(response, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<TValue> DeserializeRequiredAsync<TValue>(
        HttpContent content,
        CancellationToken cancellationToken)
        where TValue : notnull
    {
        var value = await content
                          .ReadFromJsonAsync<TValue>(cancellationToken)
                          .ConfigureAwait(false);

        return value ?? throw new JsonException(ErrorMessages.HttpResponseBodyRequired);
    }
    
    private static async Task<Option<TValue>> DeserializeOptionalAsync<TValue>(
        HttpContent content,
        CancellationToken cancellationToken)
        where TValue : notnull
    {
        var value = await content
                          .ReadFromJsonAsync<TValue>(cancellationToken)
                          .ConfigureAwait(false);

        return value is null
            ? Option<TValue>.None
            : value;
    }

    private static async Task<HttpError> DeserializeHttpErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var responseBody = await response.Content
            .ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false);

        return new HttpError(response.StatusCode, responseBody);
    }
}
