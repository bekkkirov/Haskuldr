using System.Net;

namespace Haskuldr.Http;

/// <summary>
/// Represents a non-successful HTTP response.
/// </summary>
public sealed record HttpError(
    HttpStatusCode StatusCode,
    string Response);